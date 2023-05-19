
$.fn.extend(
{
    list2Columns: function (numCols) {
        var listItems = $(this).find('li'); /* get the list data */
        var listHeader = $(this);
        var numListItems = listItems.length;
        var numItemsPerCol = Math.ceil(numListItems / numCols); /* divide by the number of columns requires */
        var currentColNum = 1, currentItemNumber = 1, returnHtml = '', i = 0;


        /* append the columns */
        for (i = 1; i <= numCols; i++) {
            $(this).parent().append('<ul class="column list-column-' + i + '"></ul>');
        }

        /* append the items to the columns */
        $.each(listItems, function (i, v) {
            if (currentItemNumber <= numItemsPerCol) {
                currentItemNumber++;
            }
            else {
                currentItemNumber = 1;
                currentColNum++;
            }
            $('.list-column-' + currentColNum).append(v);
        });
        $(this).remove(); /*clean previous content */
    }
});