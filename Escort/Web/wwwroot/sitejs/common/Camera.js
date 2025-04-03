const permissionBtn = document.getElementById('permission-btn');
const statusMessage = document.getElementById('status-message');

let cameraGrantedMessage;
let cameraDeniedMessage;
let cameraNotDetected;
let enableCamera;

// Initial check when page loads
window.addEventListener('load', async () => {
    try {
        // First check if camera exists
        const hasCamera = await checkCameraExists();
        if (!hasCamera) {
            throw new Error('No camera found');
        }
       
        // Check browser's persisted permission state
        const permissionStatus = await navigator.permissions.query({ name: 'camera' });

        if (permissionStatus.state === 'granted') {
            // If already granted, hide button and show status message
            updateUI('granted');
        } else if (permissionStatus.state === 'denied') {
            // If permission is denied, hide button and show denied message
            updateUI('denied');
        } else {
            // If permission is still a prompt (not granted or denied yet), show the button
            updateUI('prompt');
        }

    } catch (error) {
        handleError(error);
    }
});

// Button click handler
permissionBtn.addEventListener('click', async () => {
    try {
        statusMessage.textContent = '';

        // Request camera access
        const stream = await navigator.mediaDevices.getUserMedia({ video: true });

        // Immediately release camera (we only need to check permission, not use the camera)
        stream.getTracks().forEach(track => track.stop());

        // Update UI and hide button after permission granted
        updateUI('granted');

    } catch (error) {
        if (error.name === 'NotAllowedError') {
            // If permission is explicitly denied (blocked by user)
            statusMessage.textContent = cameraDeniedMessage;
            statusMessage.className = 'error';
            updateUI('denied'); // Hide button
        }
        else if (error.name === 'NotReadableError')
        {
            statusMessage.textContent = cameraGrantedMessage;
            statusMessage.className = 'success';
            updateUI('granted'); // Hide button
        }
        else
        {
            handleError(error);
        }
    }
});

async function checkCameraExists() {
    const devices = await navigator.mediaDevices.enumerateDevices();
    return devices.some(device => device.kind === 'videoinput');
}

function updateUI(permissionState) {
    if (permissionState === 'granted') {
        statusMessage.textContent = cameraGrantedMessage;
        statusMessage.className = 'success';
    } else if (permissionState === 'denied') {
        statusMessage.textContent = cameraDeniedMessage;
        statusMessage.className = 'error';
    } else {
        statusMessage.textContent = enableCamera;
        statusMessage.className = 'error';
    }
}

function handleError(error) {
    statusMessage.textContent = error.message === 'No camera found'
        ? cameraNotDetected
        : `${error.message}`;
    statusMessage.className = 'error';
}
