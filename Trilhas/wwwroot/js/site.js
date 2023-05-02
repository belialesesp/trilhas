function intVal(str) {
    str = parseInt(str);
    return (isNaN(str)) ? 0 : str;
}

String.prototype.trim = function () {
    return this.replace(/^\s*/, "").replace(/\s*$/, "");
}
