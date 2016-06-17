function mapFunction() {
    for (var i = 0; i < this.tags.length; i++) {
        emit(this.tags[i], 1);
    }
};

function reduceFunction(key, values) {
    var sum = 0;
    for (var i in values) {
        sum += values[i];
    }
    return sum;
}

(function () {
    db.posts.mapReduce(mapFunction, reduceFunction, { out: "postCountByTag" });
})();

function randomiseTags() {
    db.postCountByTag.find({}).forEach(function (doc) {
    doc.random = Math.floor((Math.random()*1000));
    db.postCountByTag.save(doc);
 });
}

