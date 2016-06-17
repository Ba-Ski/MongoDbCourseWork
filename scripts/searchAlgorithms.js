db.system.js.save(
   {
       _id: "levenshtein",
       value: function (s1, s2, costs) {
           var i, j, l1, l2, flip, ch, chl, ii, ii2, cost, cutHalf;
           l1 = s1.length;
           l2 = s2.length;

           costs = costs || {};
           var cr = costs.replace || 1;
           var cri = costs.replaceCase || costs.replace || 1;
           var ci = costs.insert || 1;
           var cd = costs.remove || 1;

           cutHalf = flip = Math.max(l1, l2);

           var minCost = Math.min(cd, ci, cr);
           var minD = Math.max(minCost, (l1 - l2) * cd);
           var minI = Math.max(minCost, (l2 - l1) * ci);
           var buf = new Array((cutHalf * 2) - 1);

           for (i = 0; i <= l2; ++i) {
               buf[i] = i * minD;
           }

           for (i = 0; i < l1; ++i, flip = cutHalf - flip) {
               ch = s1[i];
               chl = ch.toLowerCase();

               buf[flip] = (i + 1) * minI;

               ii = flip;
               ii2 = cutHalf - flip;

               for (j = 0; j < l2; ++j, ++ii, ++ii2) {
                   cost = (ch === s2[j] ? 0 : (chl === s2[j].toLowerCase()) ? cri : cr);
                   buf[ii + 1] = Math.min(buf[ii2 + 1] + cd, buf[ii] + ci, buf[ii2] + cost);
               }
           }
           return buf[l2 + cutHalf - flip];
       }
   }
)

db.system.js.save(
   {
       _id: "DL",
       value: function (source, target) {
           if (!source || source.length === 0)
               if (!target || target.length === 0)
                   return 0;
               else
                   return target.length;
           else if (!target)
               return source.length;
           var sourceLength = source.length;
           var targetLength = target.length;
           var score = [];
           var INF = sourceLength + targetLength;
           score[0] = [INF];
           for (var i = 0 ; i <= sourceLength ; i++) { score[i + 1] = []; score[i + 1][1] = i; score[i + 1][0] = INF; }
           for (var i = 0 ; i <= targetLength ; i++) { score[1][i + 1] = i; score[0][i + 1] = INF; }
           var sd = {};
           var combinedStrings = source + target;
           var combinedStringsLength = combinedStrings.length;
           for (var i = 0 ; i < combinedStringsLength ; i++) {
               var letter = combinedStrings[i];
               if (!sd.hasOwnProperty(letter))
                   sd[letter] = 0;
           }
           for (var i = 1 ; i <= sourceLength ; i++) {
               var DB = 0;
               for (var j = 1 ; j <= targetLength ; j++) {
                   var i1 = sd[target[j - 1]];
                   var j1 = DB;
                   if (source[i - 1] == target[j - 1]) {
                       score[i + 1][j + 1] = score[i][j];
                       DB = j;
                   }
                   else
                       score[i + 1][j + 1] = Math.min(score[i][j], Math.min(score[i + 1][j], score[i][j + 1])) + 1;
                   score[i + 1][j + 1] = Math.min(score[i + 1][j + 1], score[i1][j1] + (i - i1 - 1) + 1 + (j - j1 - 1));
               }
               sd[source[i - 1]] = i;
           }
           return score[sourceLength + 1][targetLength + 1];
       }
   }
)

db.system.js.save(
   {
       _id: "Hamming",
       value: function (strand1, strand2) {
           function shortestStrand(strand1, strand2) {
               if (strand1.length > strand2.length) {
                   return strand2;
               }
               else {
                   return strand1
               };
           };
           function strandCounting(shortest, splitStrand1, splitStrand2) {
               var count = 0
               for (var i = 0; i < shortest.length; i++) {
                   if (splitStrand1[i] != splitStrand2[i]) {
                       count += 1
                   };
               };
               return count;
           };
           var splitStrand1 = strand1.split("");
           var splitStrand2 = strand2.split("");
           var shortest = shortestStrand(splitStrand1, splitStrand2)
           var result = strandCounting(shortest, splitStrand1, splitStrand2);
           return result;
       }
   }
)

