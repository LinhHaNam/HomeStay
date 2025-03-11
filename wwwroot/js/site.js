

const urlParams = new URLSearchParams(window.location.search);
const message = urlParams.get('message');
if (message) {
    document.getElementById('notification').innerHTML = message
    document.getElementById('notification').hidden = false
}

setTimeout(() => {
    document.getElementById('notification').hidden = true
}, 5000)