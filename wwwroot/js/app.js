function scrollToDate(containerId, dateCellId) {
    // Add a small delay to ensure the DOM is fully rendered and sized
    setTimeout(() => {
        const container = document.getElementById(containerId);
        const dateCell = document.getElementById(dateCellId);

        if (container && dateCell) {
            // Calculate the position to center the date cell
            const containerWidth = container.offsetWidth;
            const cellWidth = dateCell.offsetWidth;
            const scrollLeft = dateCell.offsetLeft - (containerWidth / 2) + (cellWidth / 2);

            // Animate the scroll for a smoother effect
            container.scrollTo({
                left: scrollLeft,
                behavior: 'smooth'
            });
        }
    }, 150); // Increased delay slightly for more reliability
}