var places = [
    "St-Petersburg", "Moscow", "Rostov", "Vladyvostok", "Kazan",
    "Minsl", "Brest", "Warsaw", "Yekaterinburg", "Sochi",
    "New York", "Berlin", "London", "Smolensk", "Habarovsk",
    "Stockholm", "Vladimir", "Kiev", "Paris", "Pushkin", "Omsk",
    "Tomsk", "Novosibirsk", "Krasnodar", "Volgograd", "Saratov",
    "Samara", "Tyumen", "Tver", "Simferopol", "Sevastopol", "Krasnyarsk",
    "Murmansk", "Astrakhan"
]

var firstName = ["Runny", "Buttercup", "Dinky", "Stinky", "Crusty",
    "Greasy", "Gidget", "Cheesypoof", "Lumpy", "Wacky", "Tiny", "Flunky",
    "Fluffy", "Zippy", "Doofus", "Gobsmacked", "Slimy", "Grimy", "Salamander",
    "Oily", "Burrito", "Bumpy", "Loopy", "Snotty", "Irving", "Egbert", "James",
    "Waffer", "Lilly", "Rugrat", "Sand", "Fuzzy", "Kitty",
    "Puppy", "Snuggles", "Rubber", "Stinky", "Lulu", "Lala", "Sparkle", "Glitter",
    "Silver", "Golden", "Rainbow", "Cloud", "Rain", "Stormy", "Wink", "Sugar",
    "Twinkle", "Star", "Halo", "Angel"];

var lastName = ["Snicker", "Buffalo", "Gross", "Bubble", "Sheep",
    "Corset", "Toilet", "Lizard", "Waffle", "Kumquat", "Burger", "Chimp", "Liver",
    "Gorilla", "Rhino", "Emu", "Pizza", "Toad", "Gerbil", "Pickle", "Tofu",
    "Chicken", "Potato", "Hamster", "Lemur", "Vermin", "Smith", "Brown", "Ihateusa",
    "Obama", "Gay", "Kfc", "Nigga", "Chef", "Cartman"];

function randomDate(start, end) {
    return new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
}

function randomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function randomNickFormDB(count) {
    var randDate = randomDate(new Date(1905, 1, 1), new Date());
    var res = db.users.findOne({ registerDate: { $gt: randDate } });
    if (res == null)
        res = db.users.findOne({ registerDate: { $lt: randDate } });
    return res.nick;
}
function randomTagFormDB(count) {
    var randNum = randomInt(0, 1000);
    var res = db.postCountByTag.findOne({ random: { $gt: randNum } });
    if (res == null)
        res = db.postCountByTag.findOne({ random: { $lt: randNum } });
    return res._id;
}

function createComments(articleDate, usersCount) {
    var commentsCount = randomInt(1, 10);
    var comments = [];

    for (var j = 0; j < commentsCount; j++) {
        var year = articleDate.getFullYear();
        var month = articleDate.getMonth() + randomInt(0, 12);
        if (month > 12) {
            year++;
            month = month % 12;
        }
        var day = articleDate.getDay() + randomInt(0, 29)
        if (day > 30) {
            month++;
            day = day % 30;
        }

        var commentDate = new Date(year, month, day)
        comments[j] = {
            author: randomNickFormDB(usersCount),
            content: "boring content",
            date: commentDate,
            nestingLevel: 1
        }
    }
    return comments;
}

function createTags() {
    var tagsCount = randomInt(1, 4);
    var tags = [];
    var map = [];
    var tag;
    for (var j = 0; j < tagsCount; j++) {
        do {
            tag = randomTagFormDB(db.postCountByTag.count());
        } while (tag in map);
        map[tag] = 1;
        tags[j] = tag;
    }
    return tags;
}

function generateUsers() {
    for (var i = 0; i < 2100000; i++) {
        var n = Math.round(Math.random() * 1000);
        var m = Math.round(Math.random() * 1000);
        var k = Math.round(Math.random() * 1000);
        db.users.insert(
            {
                nick: "cooolGuy" + i,
                name: firstName[randomInt(0, firstName.length - 1)] + " " + lastName[randomInt(0, lastName.length - 1)],
                from: places[randomInt(0, places.length)],
                registerDate: randomDate(new Date(1600, 1, 1), new Date()),
            }
        )
    }
}

function generatePosts() {
    var usersCount = db.users.count();

    for (var i = 45000; i < 1000000; i++) {
        var author = randomNickFormDB(usersCount);
        var userRegisterdDate = db.users.findOne({ nick: author }).registerDate;
        var year = userRegisterdDate.getFullYear() + randomInt(0, 5);
        var month = userRegisterdDate.getMonth() + randomInt(0, 12);

        if (month > 12) {
            year++;
            month = month % 12;
        }

        var day = userRegisterdDate.getDay() + randomInt(0, 29)
        if (day > 30) {
            month++;
            day = day % 30;
        }

        var articleDate = new Date(year, month, day);

        db.posts.insert(
            {
                title: "awesome title " + i,
                author: author,
                content: "boring article",
                comments: createComments(articleDate, usersCount),
                tags: createTags(),
                date: articleDate,
                url: "boring url"
            }
        )
        usersCount++;
    }
}
