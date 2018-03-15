var array = [];
function calculate_sub(item) {
    var id_hour = "hour" + item.toString();
    var time = parseFloat(document.getElementById(id_hour).value);
    var id_rate = "rate" + item.toString();
    var rate = parseFloat(document.getElementById(id_rate).value);
    var hour = Math.floor(time);
    var minutes = Math.abs(((time - hour) * 100) / 60);
    var sub = (hour * rate) + (minutes * rate);
    array.push(sub);
    sub = sub.toFixed(2);
    var id_total = "total" + item.toString();
    document.getElementById(id_total).value = sub;
}
function calculate_total() {
    var total = 0;
    for (var i = 0; i < array.length; i++) {
        total += array[i];
    }
    total = total.toFixed(2);
    $('#Grand_Total').val(total);
}