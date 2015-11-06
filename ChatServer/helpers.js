exports.html_escape = function (html) {
    return String(html).replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
};
exports.html_unescape = function (html) {
    return String(html).replace(/&amp;/g, '&').replace(/&quot;/g, '"').replace(/&#39;/g, '\'').replace(/&lt;/g, '<').replace(/&gt;/g, '>');
};
exports.to_jstr = typeof JSON !== "undefined" ? JSON.stringify : function (obj) {
    var arr = [];
    var func = helpers.to_jstr === "function" ? helpers.to_jstr : exports.to_jstr;
    $.each(obj, function (key, val) {
        var next = key + ": ";
        next += (typeof val === "object" ? func(val) : val);
        arr.push(next);
    });
    return "{ " + arr.join(", ") + " }";
};