$(function () {
    $body = (window.opera) ? (document.compatMode == "CSS1Compat" ? $('html') : $('body')) : $('html,body');
    $animating = false;
    $('.fixedBtn .top').click(function () {
        if ($animating == false) {
            $body.animate({ scrollTop: 0 }, 500);
            $animating = true;
        }
        return false;
    });
    $(window).scroll(function () {
        if ($('.fixedBtn').offset().top > 500) {
            $('.fixedBtn .top').css('display', 'inline-block');
        } else {
            $('.fixedBtn .top').css('display', 'none');
            $animating = false;
        }
    });
    //var fixedBtn = $('.fixedBtn'),
	//	feedback = $('.feedback', fixedBtn),
	//	feedbackCnt = $('.feedbackCnt'),
	//	sendBtn = $('.button', fixedBtn),
	//	content = $('textarea', fixedBtn),
	//	feedbackTips = $('.feedbackTips', fixedBtn),
	//	FBCLASS = 'fb-open',
	//	close = function () {
	//	    fixedBtn.animate({
	//	        left: '-=350'
	//	    })
	//	    feedbackCnt.removeClass(FBCLASS);
	//	},
	//	open = function () {
	//	    feedbackTips.hide();
	//	    feedbackCnt.show();
	//	    fixedBtn.animate({
	//	        left: '+=350'
	//	    });
	//	    feedbackCnt.addClass(FBCLASS);
	//	};
    //feedback.length && feedback.on('click', function (e) {
    //    e.preventDefault();
    //    feedbackCnt.hasClass(FBCLASS) ? close() : open();
    //});
});