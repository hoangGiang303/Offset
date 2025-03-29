function toggleDropdown() {
    document.querySelector('.dropdown').classList.toggle('active');
}

function selectOption(element) {
    document.querySelector('.dropdown-btn').innerHTML = element.innerText + ' <i class="fa-solid fa-sort-down"></i>';
    document.querySelector('.dropdown').classList.remove('active');
}

// Ẩn dropdown nếu click ra ngoài
document.addEventListener("click", function (event) {
    const dropdown = document.querySelector(".dropdown");
    if (!dropdown.contains(event.target)) {
        dropdown.classList.remove("active");
    }
});