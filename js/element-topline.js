(
  function($)
  {
    $(window).load(
        function() {
          setTimeout(show, 100);
      }
    );

    function show() {
      $('#topLine').trigger('start');
    };

    var colours = ['#64A363', '#2E8965', '#436A55'];

    $(
      function(){ // document.ready
        $('#topLine').each(
          function(event) {
            var length = colours.length;
            var size = Math.max(length , Math.ceil( $(this).children().width() / 80 ));
            var divs = '';
            var arrColours = [];
            var setColours = [];
            var helpArr = [];

            for (var i = 0, l = size; i < l; i++) {
              var n = i % length;
              arrColours[n] = n;
              setColours[i] = n;
              helpArr[i] = i;
              divs += '<div style="background-color:'+ colours[n] +'"></div>'
            }

            var $divs = $(divs).appendTo( $(this).children() );

            var shuffle = function(v) {
                for(var j, x, i = v.length; i; j = parseInt(Math.random() * i), x = v[--i], v[i] = v[j], v[j] = x);
                return v;
            };

            var shuffleArr = shuffle(helpArr.slice(0))

            var anim = function() {
              var r = shuffleArr.shift();

              var arr = ' ' + arrColours.join(' ');

              // get color
              arr = arr.replace( ' ' + setColours[r], '' );
              if (r > 0) arr = arr.replace( ' ' + setColours[r-1], '' );
              if (r < setColours.length - 1) arr = arr.replace( ' ' + setColours[r + 1], '' );

              arr = arr.slice(1).split(' ');

              setColours[r] = shuffle(arr).shift();
              $divs.eq(r).css('background-color', colours[ setColours[r] ] );

              if(!shuffleArr.length) {
                shuffleArr = shuffle(helpArr.slice(0))
              }

              setTimeout(anim, 250 + ( Math.random() * 250 ))
            }

            $(this).on('start', anim);
          }
        );
      }
    );
  }
)
(jQuery)
