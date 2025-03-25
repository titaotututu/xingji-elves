//代办事项
async function loadTodoList() {
    let response = await fetch("http://localhost:5000/api/todo");
    let todos = await response.json();

    let content = "<h2>待办事项</h2><ul>";
    todos.forEach(todo => {
        content += `<li>${todo.Description} - ${todo.IsCompleted ? "✅ 已完成" : "❌ 未完成"}</li>`;
    });
    content += "</ul>";

    document.getElementById("content").innerHTML = content;
}
