function submitted() {
    var inputBox = document.getElementById("message");
    var message = inputBox.value;
    var mode = $("input[type='radio']:checked").attr("value");
    var submitButton = document.getElementById("submitButton");

    message = "{Text:\"" + message + "\", Config:{Display: \"" + mode + "\"}}";

    inputBox.setAttribute('disabled', 'true');
    submitButton.setAttribute('disabled', 'true');

    $.ajax({
        url: "cbw/Channels(0)/Captions",
        type: "POST",
        data: message,
        headers: {
            "Content-Type": "application/json"
        },
        success: function() {
            inputBox.value = "发送成功！下一条要等三秒钟才能发哦~";

            setTimeout("document.getElementById('message').removeAttribute('disabled');" +
                "document.getElementById('submitButton').removeAttribute('disabled');" + 
                "document.getElementById('message').value = '';" +
                "document.getElementById('message').setAttribute('placeholder', '吐槽吧，骚年！\\(^-^)/');", 3000);
        },
        error: function () {
            document.getElementById('message').removeAttribute('disabled');
            document.getElementById('submitButton').removeAttribute('disabled');
            alert("出错啦~");
        }
    });
}