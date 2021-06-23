import validator from "validator";

const signUpModal = document.getElementById('signupModal');
const closeBtn = document.getElementById('closeBtn');
const newAccountBtn = document.getElementById('newAccountBtn');
const username = document.getElementById('#username');
const loginBtn = document.getElementById('loginBtn');
loginBtn.addEventListener('click', (e)=>{
    e.preventDefault();
    
});
window.addEventListener('keyup', escPress);
function closeModal(modal) {
    modal.style.display = "none";
}
function openModal(modal) {
    modal.style.display = "flex";
}

function escPress(e) {
    if (e.which == '27') {
        closeModal(signUpModal);
    }
}
newAccountBtn.onclick = function () {
    openModal(signUpModal);
}
closeBtn.onclick = function () {
    closeModal(signUpModal);
}