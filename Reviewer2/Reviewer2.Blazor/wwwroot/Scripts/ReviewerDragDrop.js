const initialized = new WeakMap();

function log(...args) {
    console.log("[DragDrop]", ...args);
}

// =====================
// REVIEWER POOL (drag source)
// =====================
export function initReviewerPool(el, dotNetRef) {
    if (!el || initialized.has(el)) return;

    const list = el.querySelector("ul");
    if (!list) return;

    new Sortable(list, {
        group: {
            name: "reviewers",
            pull: "clone", // clone from pool
            put: false
        },
        sort: false,
        animation: 150,
        onStart: (evt) => {
            const reviewerId = evt.item.dataset.reviewerId;
            if (dotNetRef && reviewerId) {
                dotNetRef.invokeMethodAsync("OnDragStartJS", reviewerId);
            }
        },
        // IMPORTANT: disable DOM insertion entirely for clones
        setData: function(dataTransfer, dragEl) {
            // store reviewerId in the drag event
            dataTransfer.setData("text/plain", dragEl.dataset.reviewerId);
        },
    });

    initialized.set(el, list);
}

// =====================
// PAPER COLUMN (drop target)
// =====================
export function initPaperColumn(el, dotNetRef, paperId) {
    if (!el || initialized.has(el)) return;

    const list = el.querySelector("ul");
    if (!list) return;

    new Sortable(list, {
        group: {
            name: "reviewers",
            pull: false,
            put: true
        },
        sort: false, // no reordering
        animation: 150,
        ghostClass: "drag-ghost",
        chosenClass: "drag-chosen",
        onAdd: (evt) => {
            const reviewerId = evt.item.dataset.reviewerId;

            // Immediately remove DOM insertion (Blazor owns UI)
            evt.item.remove();

            if (dotNetRef && reviewerId && paperId) {
                dotNetRef.invokeMethodAsync(
                    "OnReviewerDroppedJS",
                    paperId,
                    reviewerId
                );
            }
        },
        onMove: (evt) => {
            const reviewerId = evt.dragged.dataset.reviewerId;
            const alreadyAssigned = [...evt.to.querySelectorAll("[data-reviewer-id]")]
                .some(e => e.dataset.reviewerId === reviewerId);
            return !alreadyAssigned;
        }
    });

    initialized.set(el, list);
}

// =====================
// DESTROY
// =====================
export function destroy(el) {
    if (!el) return;

    const list = el.querySelector("ul");
    if (!list) return;

    const sortable = initialized.get(list);

    if (sortable) {
        try {
            sortable.destroy();
        } catch { }
        initialized.delete(list);
    }
}