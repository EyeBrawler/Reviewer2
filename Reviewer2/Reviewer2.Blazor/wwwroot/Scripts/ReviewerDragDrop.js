// reviewerDragDrop.js

// Keep track of initialized elements to avoid double init
const initialized = new WeakMap();

/**
 * Initialize the Reviewer Pool (drag source)
 * @param {HTMLElement} el - UL or container element
 * @param {any} dotNetRef - DotNetObjectReference
 */
export function initReviewerPool(el, dotNetRef) {
    if (!el || initialized.has(el)) return;

    const list = el.querySelector("ul"); 

    if (!list) return;

    const sortable = new Sortable(list, {
        group: {
            name: "reviewers",
            pull: "clone",   // clone reviewers when dragging
            put: false       // cannot drop into pool
        },
        sort: false,          // do not reorder pool
        animation: 150,
        ghostClass: "drag-ghost",
        chosenClass: "drag-chosen",
        dragClass: "drag-dragging",

        onStart: (evt) => {
            const reviewerId = evt.item.dataset.reviewerId;

            if (dotNetRef && reviewerId) {
                dotNetRef.invokeMethodAsync("OnDragStartJS", reviewerId);
            }
        }
    });

    initialized.set(el, sortable);
}

/**
 * Initialize a Paper Column (drop target)
 * @param {HTMLElement} el - Drop container
 * @param {any} dotNetRef - DotNetObjectReference
 * @param {string} paperId - Guid string
 */
export function initPaperColumn(el, dotNetRef, paperId) {
    if (!el || initialized.has(el)) return;

    const sortable = new Sortable(el, {
        group: {
            name: "reviewers",
            pull: false,
            put: true
        },
        animation: 150,
        ghostClass: "drag-ghost",
        chosenClass: "drag-chosen",

        onAdd: (evt) => {
            const reviewerId = evt.item.dataset.reviewerId;

            // IMPORTANT: revert DOM change immediately
            // Blazor owns rendering
            evt.item.remove();

            if (dotNetRef && reviewerId && paperId) {
                dotNetRef.invokeMethodAsync("OnReviewerDroppedJS", paperId, reviewerId);
            }
        },

        onMove: (evt) => {
            // Optional: prevent dropping duplicates
            const reviewerId = evt.dragged.dataset.reviewerId;

            if (!reviewerId) return true;

            const alreadyAssigned = [...evt.to.querySelectorAll("[data-reviewer-id]")]
                .some(e => e.dataset.reviewerId === reviewerId);

            if (alreadyAssigned) {
                return false; // block drop
            }

            return true;
        }
    });

    initialized.set(el, sortable);
}

/**
 * Destroy a sortable instance (important for Blazor re-renders)
 */
export function destroy(el) {
    if (!el) return;

    const sortable = initialized.get(el);
    if (sortable) {
        try {
            sortable.destroy();
        } catch (e) {
            console.warn("Failed to destroy sortable", e);
        }

        initialized.delete(el);
    }
}