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

    const sortable = new Sortable(list, {
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

    initialized.set(el, sortable);
}

// =====================
// PAPER COLUMN (drop target for reviewers)
// =====================
export function initPaperColumn(el, dotNetRef, paperId) {
    if (!el || initialized.has(el)) return;

    const list = el.querySelector("ul");
    if (!list) return;

    const sortable = new Sortable(list, {
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

    initialized.set(el, sortable);
}

// =====================
// SESSION COLUMN (Drag source & Drop target for papers)
// =====================
export function initSessionColumn(el, dotNetRef, sessionName) {
    if (!el || initialized.has(el)) return;

    // Target the specific container inside the column that holds the papers
    const container = el.querySelector(".column-body");
    if (!container) return;

    const sortable = new Sortable(container, {
        group: "papers", // Shared group name allows dragging between all columns
        animation: 150,
        ghostClass: "drag-ghost",
        chosenClass: "drag-chosen",
        draggable: ".paper-drop-zone", // Ensures we drag papers, not the "Drop paper here" empty state UI

        // Fired when a paper is dropped into THIS column from another column
        onAdd: (evt) => {
            const paperId = evt.item.dataset.paperId;

            // CRITICAL FOR BLAZOR: Immediately remove the DOM node Sortable just moved.
            // Blazor maintains its own virtual DOM. We delete Sortable's DOM manipulation 
            // and let Blazor re-render the paper in the new column based on the C# state.
            evt.item.remove();

            if (dotNetRef && paperId) {
                log(`Paper ${paperId} dropped into ${sessionName}`);
                dotNetRef.invokeMethodAsync(
                    "OnPaperDroppedJS",
                    paperId,
                    sessionName
                );
            }
        },

        // Fired if you reorder papers within the SAME column
        onUpdate: (evt) => {
            // If you care about sorting order inside the session, handle it here.
            // Otherwise, you can leave this blank or disable sorting via `sort: false`.
        }
    });

    initialized.set(el, sortable);
}

// =====================
// DESTROY
// =====================
export function destroy(el) {
    if (!el) return;

    // Try to find the sortable instance on the element itself or its common child lists
    const container = el.querySelector("ul") || el.querySelector(".column-body") || el;
    const sortable = initialized.get(el) || initialized.get(container);

    if (sortable) {
        try {
            sortable.destroy();
        } catch { }
        initialized.delete(el);
        initialized.delete(container);
    }
}