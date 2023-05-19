(function($) {
    var jqCss = $.fn.css;

    var respond = function(event, element) {
        var that = $(event.target);
        var applied = true;
        var styles = window.getComputedStyle(event.target);

        $.each(element.when, function(key, value) {
            if(!value(styles[key])) {
                applied = false;
            }
        });
        if(applied) {
            $.each(element.set, function(key, value) {
                return jqCss.apply(that, [key, value]);
            });
        }
        return $(event.target);
    };

    $.fn.css = function() {
        $(this).trigger('respond');
        return jqCss.apply(this, arguments);
    };

    $.fn.respond = function() {
        var that = this;

        $(that).bind('resize', function(event) {
            $(that).trigger('respond');
        });

        $.each(arguments, function(index, element) {
            if(element.when && element.set) {
                $(that).bind('respond', function(event) {
                    respond(event, element);
                });
            }
        });
        return this;
    };
}(jQuery));
