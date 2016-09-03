$(Init);
function Init() {
    var count = 0;
    var aCls = 'active'

    var $right = $('.right');
    var $bigBall = $('.big-ball');
    var $tryAgain = $('.try-again');
    var $exploreMysteries = $('.explore-mysteries');
    var $mysteries = $('.mysteries');
    var $info = $('.info');
    var $btns = $('.btns');
    var $ball = $('.post-header-container');

    var srcBlue;
    var srcWhite;

    $tryAgain.on('click', function () {
        Reset();
        $btns.removeClass(aCls);
        $info.removeClass(aCls);
        $bigBall.removeClass(aCls);
        $mysteries.removeClass(aCls);
    }).click();

    function Reset() {
        count++;
        var html = '';
        var v9 = parseInt(Math.random() * 100 % 25);

        $bigBall.html('');
        $('<svg><use xlink:href="imgs/icons_blue.svg#' + v9 + '"></use></svg>').appendTo($bigBall);

        srcWhite = 'imgs/icons_white.svg#' + v9;

        for (var i = 0; i < 100; i++) {
            var t = parseInt(Math.random() * 100 % 25);
            var b9 = i % 9 == 0 && i != 0 && i != 90 && i != 99;
            var cls = b9 ? 'cls9' : 'cls';
            html += '<span class="' + cls + '"><span class="number">' + i + '</span><svg><use xlink:href="imgs/icons_blue.svg#' + (b9 ? v9 : t) + '"></use></svg></span>';
        }
        $right.html(html);

        $i9 = $right.find('.cls9');

        $ball.off().on('click', function () {
            $bigBall.addClass(aCls);
            $info.addClass(aCls);
            $btns.addClass(aCls);
        });

        $exploreMysteries.off().on('click', function () {
            $i9.addClass(aCls);
            $i9.find('use').attr('xlink:href', srcWhite);
            $mysteries.addClass(aCls);
            $info.removeClass(aCls);
        });

        if (count > 4) $exploreMysteries.addClass(aCls);
    }
}