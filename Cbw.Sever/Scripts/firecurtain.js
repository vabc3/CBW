function submitted() {
    var inputBox = document.getElementById("message");

    var message = inputBox.value;

    $.ajax({
        url: "http://localhost:3338/cbw/Channels(0)/Captions",
        type: "POST",
        data: "{Text:\"" + message + "\"}",
        headers: {
            "Content-Type": "application/json"
        },
        success: function() {
            inputBox.setAttribute('disabled', 'true');
            inputBox.value = "发送成功！下一条要等十秒钟才能发哦~";

            setTimeout("document.getElementById('message').removeAttribute('disabled')", 10000);
        },
        error: function() {
            alert("出错啦~");
        }
    });
}