$(document).ready(function() {
	$('#clickme').toggle(function() {
		$(this).parent().animate({left:'0px'});
	}, function() {
		$(this).parent().animate({left:'-160px'});
	});

	// style 

	$(".regular").click(function() {
		$("link.responsive").attr("href", "css/skeleton.css");
		$.removeCookie('responsive');
		return false;
	});
	$(".skwide").click(function() {
		$("link.responsive").attr("href", "css/skeleton-wide.css");
		$.cookie("responsive", "css/skeleton-wide.css");
		return false;
	});
	$(".swwide").click(function() {
		$("#wrap").removeClass('boxed').addClass('wide');
		$.removeCookie('boxed');
		return false;
	});
	$(".swboxed").click(function() {
		$("#wrap").removeClass('wide').addClass('boxed');
		$.cookie('boxed', 'boxed');
		return false;
	});

	// color

	$(".yellow").click(function() {
		$("link.color").attr("href", "css/color/yellow.css");
		$.cookie("color", "css/color/yellow.css");
		return false;
	});
	$(".orange").click(function() {
		$("link.color").attr("href", "css/color/orange.css");
		$.cookie("color", "css/color/orange.css");
		return false;
	});
	$(".red").click(function() {
		$("link.color").attr("href", "css/color/red.css");
		$.cookie("color", "css/color/red.css");
		return false;
	});
	$(".green").click(function() {
		$("link.color").attr("href", "css/color/green.css");
		$.cookie("color", "css/color/green.css");
		return false;
	});
	$(".blue").click(function() {
		$("link.color").attr("href", "css/color/blue.css");
		$.cookie("color", "css/color/blue.css");
		return false;
	});

	//patterns

	$(".pat1").click(function() {
		$("body").removeClass('wood2 ticks greyzz grayjean brickwall cream tweed oliva binding').addClass('wood');
		return false;
	});
	$(".pat2").click(function() {
		$("body").removeClass('wood ticks greyzz grayjean brickwall cream tweed oliva binding').addClass('wood2');
		return false;
	});
	$(".pat3").click(function() {
		$("body").removeClass('wood wood2 greyzz grayjean brickwall cream tweed oliva binding').addClass('ticks');
		return false;
	});
	$(".pat4").click(function() {
		$("body").removeClass('wood wood2 ticks grayjean brickwall cream tweed oliva binding').addClass('greyzz');
		return false;
	});
	$(".pat5").click(function() {
		$("body").removeClass('wood wood2 ticks greyzz brickwall cream tweed oliva binding').addClass('grayjean');
		return false;
	});
	$(".pat6").click(function() {
		$("body").removeClass('wood wood2 ticks greyzz grayjean cream tweed oliva binding').addClass('brickwall');
		return false;
	});
	$(".pat7").click(function() {
		$("body").removeClass('wood wood2 ticks greyzz grayjean brickwall tweed oliva binding').addClass('cream');
		return false;
	});
	$(".pat8").click(function() {
		$("body").removeClass('wood wood2 ticks greyzz grayjean brickwall cream oliva binding').addClass('tweed');
		return false;
	});
	$(".pat9").click(function() {
		$("body").removeClass('wood wood2 ticks greyzz grayjean brickwall cream tweed binding').addClass('oliva');
		return false;
	});
	$(".pat10").click(function() {
		$("body").removeClass('wood wood2 ticks greyzz grayjean brickwall cream tweed oliva').addClass('binding');
		return false;
	});
});