// reviewerDragDrop.js

const initialized = new WeakMap();

function log(...args) {
    console.log("[DragDrop]", ...args);
}

// =====================
// REVIEWER POOL
// =====================
export function initReviewerPool(el, dotNetRef) {
    log("INIT POOL", el);

    const list = el.querySelector("ul");
    log("POOL LIST FOUND:", list);

    if (!list || initialized.has(list)) return;

    const sortable = new Sortable(list, {
        group: {
            name: "reviewers",
            pull: "clone",
            put: false
        },
        sort: false,
        animation: 150,

        onStart: (evt) => {
            const reviewerId = evt.item.dataset.reviewerId;

            log("DRAG START", reviewerId);

            if (dotNetRef && reviewerId) {
                dotNetRef.invokeMethodAsync("OnDragStartJS", reviewerId);
            }
        }
    });

    initialized.set(list, sortable);
}

// =====================
// PAPER COLUMN
// =====================
export function initPaperColumn(el, dotNetRef, paperId) {
    log("INIT PAPER", el, "paperId:", paperId);

    const list = el.querySelector("ul");
    if (!list || initialized.has(list)) return;

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

            log("ON ADD (DROP)", reviewerId);

            // Let Blazor own state
            evt.item.remove();

            if (dotNetRef && reviewerId && paperId) {
                dotNetRef.invokeMethodAsync("OnReviewerDroppedJS", paperId, reviewerId);
            }
        },

        onMove: (evt) => {
            const reviewerId = evt.dragged.dataset.reviewerId;

            const alreadyAssigned = [...evt.to.querySelectorAll("[data-reviewer-id]")]
                .some(e => e.dataset.reviewerId === reviewerId);

            if (alreadyAssigned) {
                log("BLOCKING DUPLICATE", reviewerId);
                return false;
            }

            return true;
        }
    });

    initialized.set(list, sortable);
}

// =====================
// DESTROY
// =====================
export function destroy(el) {
    if (!el) return;

    const list = el.querySelector("ul") ?? el;
    const sortable = initialized.get(list);

    if (sortable) {
        log("DESTROYING SORTABLE");

        try {
            sortable.destroy();
        } catch (e) {
            console.warn("Failed to destroy sortable", e);
        }

        initialized.delete(list);
    }
}