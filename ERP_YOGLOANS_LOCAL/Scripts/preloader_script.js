
// script.js

var spinner = new Spinner({
    lines: 20,
    length: 10,
    width: 5,
    radius: 10,
    scale: 1,
    corners: 1,
    color: '#808080',
    opacity: 0,
    rotate: 0,
    direction: 1,
    speed: 1,
    trail: 60,
    fps: 20,
    zIndex: 2e9,
    className: 'spinner',
    top: '50%',
    left: '50%',
    shadow: false,
    hwaccel: false,
    position: 'absolute'
});

// Function to show the loading indicator
function showLoadingIndicator() {   
    // Show the spinner
    spinner.spin(document.getElementById('loadingOverlay'));
    // Show the overlay
    document.getElementById('loadingOverlay').style.display = 'flex';
}

// Function to hide the loading indicator
function hideLoadingIndicator() {
    // Hide the overlay
    document.getElementById('loadingOverlay').style.display = 'none';
    // Stop the spinner
    spinner.stop();
}
