function keypress(event) {
    if (event.keyCode == 13) {
        submitted();
    }
}

function submitted() {
    var inputBox = document.getElementById("message");
    var message = inputBox.value;
    var mode = $("input[type='radio']:checked").attr("value");

    var payload = "{Text:\"" + message + "\", Config:{Display: \"" + mode + "\"}}";

    inputBox.setAttribute('disabled', 'true');

    $.ajax({
        url: "cbw/Channels(0)/Caption",
        type: "POST",
        data: payload,
        headers: {
            "Content-Type": "application/json"
        },
        success: function () {
            inputBox.value = "发送成功！下一条要等三秒钟才能发哦~";

            setTimeout("document.getElementById('message').removeAttribute('disabled');" +
                "document.getElementById('message').value = '';" +
                "document.getElementById('message').setAttribute('placeholder', '吐槽吧，骚年！\\(^-^)/');", 3000);
        },
        error: function () {
            inputBox.value = "发送失败了T_T。。没事，等三秒钟再试试~";

            setTimeout("document.getElementById('message').removeAttribute('disabled');" +
                "document.getElementById('message').value = '" + message + "';", 3000);
        }
    });
}