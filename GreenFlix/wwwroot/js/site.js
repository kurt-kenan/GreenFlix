// Back to top button
document.addEventListener('DOMContentLoaded', function () {
    // Get the button
    let backToTopButton = document.getElementById('btn-back-to-top');

    // When the user scrolls down 20px from the top, show the button
    window.onscroll = function () {
        if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
            backToTopButton.style.display = "block";
            backToTopButton.classList.add('animate__animated', 'animate__fadeIn');
        } else {
            backToTopButton.style.display = "none";
        }
    };

    // When the user clicks on the button, scroll to the top of the document
    backToTopButton.addEventListener('click', function () {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });
});