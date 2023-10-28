function getPageReferrer() {
    return document.referrer;
} 

function addPreviousPage() {
    window.history.pushState({ prevUrl: window.location.href }, null, null);
}
function getPreviousPage() {
    return window.history.state.prevUrl
}