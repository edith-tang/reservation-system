
(function ($) {

    var defaults = {
        sittingDate: '2019-01-01',
        sittingName: 'Breakfast',
        timeslots: [{ id: 1, startTime: "8:00:00", endTime: "9:00:00" }, { id: 2, startTime: "9:00:00", endTime: "10:00:00" },  { id: 3, startTime: "10:00:00", endTime: "11:00:00" }],
        tables: [{ id: 1, Name: "A1" }, { id: 2, Name: "A2" }],
        sittingunits: [
            {id:1, timeslotid:1,tableid:1 },
            { id: 2, timeslotid: 1, tableid: 2},
            { id: 3, timeslotid: 2, tableid: 1},
            { id: 4, timeslotid: 2, tableid: 2},
            { id: 5, timeslotid: 3, tableid: 1},
            { id: 6, timeslotid: 3, tableid: 2}]
    }

    $.fn.scheduler = function (options) {

        //Merge the contents of options into the defaults.

        var settings = $.extend({}, defaults, options);

        return this.each(function () {

            render.calender($(this), settings);
            setSize(40,90);
            

        });



    }

    //render scheduler

    var render = {

        calender: function (container, settings) {

            var header = render.header(settings);
            var body = render.body(settings);

            container.append(header);
            container.append(body);

        },

        header: function (settings) {
            var header = $("<div></div>").addClass("header");
            var sittingDate = $("<div></div>").addClass("sittingdate").text(settings.sittingDate);
            var sittingName = $("<div></div>").addClass("sittingname").text(settings.sittingName);

            header.append(sittingDate);
            header.append(sittingName);

            return header;
        },

        body: function (settings) {
            var body = $("<div></div>").addClass("body");

            var rowHeader = render.rowHeader(settings);
            var timeslots = render.timeslots(settings);

            body.append(rowHeader);
            body.append(timeslots);


            return body;
        },


        rowHeader: function (settings) {

            var rowHeaderContainer = $("<div></div>").addClass("row-header-container");
            rowHeaderContainer.append("<div class='hour-header hour'>Schedule</div>");
            var i;
            for (i = 0; i < settings.tables.length; i++) {
                var header = $("<div></div>").addClass("row-header cell").text(settings.tables[i].Name);
                rowHeaderContainer.append(header);
            }

            return rowHeaderContainer;
        },

        timeslots: function (settings) {

            var columnHeader = $("<div></div>").addClass("column-header");
            var rowContainer = $("<div></div>").addClass("row-container");
            var rsvnContainer = $("<div></div>").addClass("rsvn-container");



            for (i = 0; i < settings.timeslots.length; i++) {
                var hour = $("<div></div>").addClass("hour").text(settings.timeslots[i].startTime);
                columnHeader.append(hour);
            }
            rowContainer.append(columnHeader);
            rowContainer.append(rsvnContainer);

            for (var i = 0; i < settings.tables.length; i++) {
                var row = $("<div></div>").addClass("row");
                rowContainer.append(row);

                
                for (var j = 0; j < settings.timeslots.length; j++) {
                    var timeslot = $("<div></div>").addClass("timeslot cell");
                    timeslot.attr("id", settings.sittingunits.find(x => x.tableid == settings.tables[i].id && x.timeslotid == settings.timeslots[j].id).id);
                        
                    row.append(timeslot);
                }
            }

            return rowContainer;



        },


    }

    // Sets the size of the scheduler timeslots
    var setSize = function (height, width) {
        $(".cell")
            .css({
                minHeight: height,
                minWidth: width,
                maxHeight: height,
                maxWidth: width
            });
        $(".hour")
            .css({
                minWidth: width,
                maxWidth: width
            });
    }




    }(jQuery));