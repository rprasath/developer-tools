// Add this to wwwroot/js/copy-functionality.js
function copyToClipboard(text, buttonId) {
    navigator.clipboard.writeText(text)
        .then(() => {
            // Change icon to checkmark temporarily
            const button = document.getElementById(buttonId);
            const origInnerHTML = button.innerHTML;
            
            button.innerHTML = `
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
            `;
            
            // Revert after 1.5 seconds
            setTimeout(() => {
                button.innerHTML = origInnerHTML;
            }, 1500);
        })
        .catch(err => {
            console.error('Failed to copy: ', err);
        });
}