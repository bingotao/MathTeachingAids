$(function () {
    var aCls = 'active';
    var $testCount = $('.test-count .item');
    var $testLevel = $('.test-level .item');
    var $btnStartTest = $('.btn-start-test');
    var $pageStart = $('.start');
    var $pageCalculate = $('.calculate');
    var $pageResult = $('.result');
    var $testNumbers = $('.test-numbers');
    var $testIndex = $('.test-content-title>span');
    var $testContent = $('.t-cnt');
    var $testAnswer = $('.t-answer');
    var $btnPr = $('.btn-pr');
    var $btnNext = $('.btn-next');
    var $btnSub = $('.btn-sub');
    var $btnCalulates = $('.calculate-panel .btn');
    var $total = $('.result-total');
    var $right = $('.result-right');
    var $wrong = $('.result-wrong');
    var $ratio = $('.result-ratio');
    var $results = $('.items');
    var $btnRetry = $('.btn-retry');

    var numbers = null;
    var tests = null;
    var cIndex = 0;

    $testCount.on('click', changeSelect);
    $testLevel.on('click', changeSelect);

    function changeSelect() {
        $(this).addClass(aCls).siblings().removeClass(aCls);
    }

    function GetSign() {
        return GetNum(10, false) % 2 ? -1 : 1;
    }

    function GetNum(multi, bSign) {
        return parseInt(Math.random() * multi) * (bSign ? GetSign() : 1);
    }

    function CreateAB(level) {
        var a = 0;
        var b = 0;
        while (a == 0 || b == 0) {
            switch (level) {
                case 0:
                    a = GetNum(10, false);
                    b = GetNum(10, true);
                    break;
                case 1:
                    a = GetNum(10, true);
                    b = GetNum(10, true);
                    break;
                case 2:
                    a = GetNum(100, true);
                    b = GetNum(100, true);
                    break;
            }
        }
        return { a: a, b: b };
    }

    function CreateTest() {
        $testNumbers.html('');

        var count = $testCount.filter('.' + aCls).data('value');
        var level = $testLevel.filter('.' + aCls).data('value');

        numbers = [];
        tests = [];
        for (var i = 0; i < count; i++) {
            var ab = CreateAB(level);
            var t = {
                index: (i + 1),
                content: ab.a + '' + (ab.b > 0 ? '+' + ab.b : (ab.b + '')) + '=',
                a: ab.a,
                b: ab.b,
                answer: ab.a + ab.b,
                yourAnser: NaN
            };
            tests.push(t);
            var $number = $('<span class="value-small">' + t.index + '</span>').data('test', t);

            $number.on('click', function () {
                SaveTest();
                var index = $(this).data('test').index - 1;
                InitTestInfo(index);
            });
            $number.appendTo($testNumbers);
            numbers.push($number);
        }

        InitTestInfo(0);
    }

    function InitTestInfo(index) {
        index = index < 0 ? 0 : index;
        index = index >= tests.length ? (tests.length - 1) : index;
        cIndex = index;
        var t = tests[cIndex];
        $testIndex.html(t.index);
        $testContent.html(t.content);
        $testAnswer.html(t.yourAnser);

        numbers.forEach(function (i) {
            i.removeClass('current');
        });

        numbers[cIndex].addClass('current');
    }

    $btnStartTest.on('click', function () {
        CreateTest();
        $pageStart.removeClass(aCls);
        $pageResult.removeClass(aCls);
        $pageCalculate.addClass(aCls);
    });

    $btnPr.on('click', function () {
        SaveTest();
        InitTestInfo(cIndex - 1);
    });

    $btnNext.on('click', function () {
        SaveTest();
        InitTestInfo(cIndex + 1);
    });

    function SaveTest() {
        var t = tests[cIndex];
        t.yourAnser = parseInt($testAnswer.html());
        if (isNaN(t.yourAnser)) {
            numbers[cIndex].removeClass(aCls);
        } else {
            numbers[cIndex].addClass(aCls);
        }
    }

    $btnCalulates.on('click', function () {
        var $this = $(this);
        var text = $testAnswer.html();
        var html = $this.html();
        if (html == '-') {
            $testAnswer.html(html);
        } else if (html == 'C') {
            $testAnswer.html('');
        } else {
            $testAnswer.html(text + html);
        }
    });

    $btnSub.on('click', function () {
        SaveTest();
        var total = tests.length;
        var right = 0;
        var wrong = 0;
        var html = '';
        for (var i = 0; i < total; i++) {
            var t = tests[i];
            var fh = true;
            if (t.answer == t.yourAnser) {
                fh = true;
                right++;
            } else {
                fh = false;
                wrong++;
            }

            html += ('<tr class="' + (!fh ? 'row-red' : '') + '"><td>' + t.index + '</td><td>' + t.content + '</td><td>' + t.answer + '</td><td>' + (isNaN(t.yourAnser) ? '未作答' : t.yourAnser) + '</td></tr>');
        }
        $total.html(tests.length);
        $right.html(right);
        $wrong.html(wrong);
        $ratio.html((right * 100 / total).toFixed(2) - 0);
        $results.html(html);
        $pageStart.removeClass(aCls);
        $pageCalculate.removeClass(aCls);
        $pageResult.addClass(aCls);
    });

    $btnRetry.on('click', function () {
        $pageStart.addClass(aCls);
        $pageCalculate.removeClass(aCls);
        $pageResult.removeClass(aCls);
    });
});