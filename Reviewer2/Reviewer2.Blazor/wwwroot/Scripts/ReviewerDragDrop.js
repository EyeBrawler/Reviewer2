// reviewerDragDrop.js

// Keep track of initialized elements to avoid double init

const initialized = new WeakMap();
function log(...args) {
    console.log("[DragDrop]", ...args);
}

/**
 * Initialize the Reviewer Pool (drag source)
 * @param {HTMLElement} el - UL or container element
 * @param {any} dotNetRef - DotNetObjectReference
 */
export function initReviewerPool(el, dotNetRef) {
    log("INIT POOL", el);

    const list = el.querySelector("ul");
    log("POOL LIST FOUND:", list);

    if (!list) return;

    log("POOL CHILD COUNT:", list.children.length);
    
    if (!el || initialized.has(el)) return;

   // const list = el.querySelector("ul"); 

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

            log("DRAG START", {
                reviewerId,
                text: evt.item.innerText,
                element: evt.item
            });

            if (dotNetRef && reviewerId) {
                dotNetRef.invokeMethodAsync("OnDragStartJS", reviewerId);
            }
        }
    });

    initialized.set(el, sortable);

    log("POOL SORTABLE CREATED", sortable);
    log("POOL REVIEWER COUNTS:", countReviewers(list));
}

/**
 * Initialize a Paper Column (drop target)
 * @param {HTMLElement} el - Drop container
 * @param {any} dotNetRef - DotNetObjectReference
 * @param {string} paperId - Guid string
 */
export function initPaperColumn(el, dotNetRef, paperId) {

    log("INIT PAPER", el, "paperId:", paperId);
    log("PAPER CHILD COUNT:", el.children.length);
    
    if (!el || initialized.has(el)) return;

    const list = el.querySelector("ul");
    if (!list) return;

    const sortable = new Sortable(list, {
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

            log("ON ADD (DROP)", {
                reviewerId,
                to: evt.to,
                from: evt.from,
                item: evt.item
            });

            // BEFORE removal
            log("BEFORE REMOVE - children:", evt.to.children.length);

            evt.item.remove();

            // AFTER removal
            log("AFTER REMOVE - children:", evt.to.children.length);
            log("PAPER REVIEWER COUNTS:", countReviewers(evt.to));

            if (dotNetRef && reviewerId && paperId) {
                log("CALLING BLAZOR", paperId, reviewerId);
                dotNetRef.invokeMethodAsync("OnReviewerDroppedJS", paperId, reviewerId);
            }
        },

        onMove: (evt) => {
            const reviewerId = evt.dragged.dataset.reviewerId;

            log("ON MOVE", {
                reviewerId,
                to: evt.to,
                from: evt.from
            });

            const alreadyAssigned = [...evt.to.querySelectorAll("[data-reviewer-id]")]
                .some(e => e.dataset.reviewerId === reviewerId);

            if (alreadyAssigned) {
                log("BLOCKING DROP - DUPLICATE", reviewerId);
                return false;
            }

            return true;
        }
    });

    initialized.set(list, sortable);
}

/**
 * Destroy a sortable instance (important for Blazor re-renders)
 */
export function destroy(el) {
    if (!el) return;

    log("DESTROY CALLED", el);

    const sortable = initialized.get(el);

    if (sortable) {
        log("DESTROYING SORTABLE", sortable);

        try {
            sortable.destroy();
        } catch (e) {
            console.warn("Failed to destroy sortable", e);
        }

        initialized.delete(el);
    } else {
        log("NO SORTABLE FOUND FOR ELEMENT");
    }
}

function countReviewers(container) {
    const ids = [...container.querySelectorAll("[data-reviewer-id]")]
        .map(e => e.dataset.reviewerId);

    const counts = {};
    ids.forEach(id => counts[id] = (counts[id] || 0) + 1);

    return counts;
}