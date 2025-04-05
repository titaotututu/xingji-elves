document.addEventListener("DOMContentLoaded", function () {
    const logoutBtn = document.getElementById("logout-btn"); 

    // 退出逻辑
    if (logoutBtn) {
        logoutBtn.addEventListener("click", function (event) {
            event.preventDefault(); // 阻止默认行为

            const userConfirmed = confirm("确定要退出吗？");

            if (userConfirmed) {
                // 清除用户数据
                localStorage.removeItem("currentUser"); // 移除当前用户信息
                sessionStorage.clear(); // 清空 sessionStorage（如果有）

                // 跳转到登录页面或首页
                window.location.href = "../index.html";
            }
        });
    }

});
