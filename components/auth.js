document.addEventListener("DOMContentLoaded", function () {
    const loginBtn = document.getElementById("login-btn");
    const registerBtn = document.getElementById("register-btn");

    if (loginBtn) {
        loginBtn.addEventListener("click", function () {
            const username = document.getElementById("username").value;
            const password = document.getElementById("password").value;

            if (!username || !password) {
                alert("请输入用户名和密码！");
                return;
            }

            const users = JSON.parse(localStorage.getItem("users")) || [];
            const user = users.find(u => u.username === username && u.password === password);

            if (user) {
                alert("登录成功！");
                localStorage.setItem("currentUser", username);
                window.location.href = "main.html";
            } else {
                alert("用户名或密码错误！");
            }
        });
    }

    if (registerBtn) {
        registerBtn.addEventListener("click", function () {
            const username = document.getElementById("username").value;
            const password = document.getElementById("password").value;

            if (!username || !password) {
                alert("请输入用户名和密码！");
                return;
            }

            const users = JSON.parse(localStorage.getItem("users")) || [];

            if (users.some(u => u.username === username)) {
                alert("用户名已被占用！");
                return;
            }

            users.push({ username, password });
            localStorage.setItem("users", JSON.stringify(users));

            alert("注册成功！请登录");
        });
    }
});
