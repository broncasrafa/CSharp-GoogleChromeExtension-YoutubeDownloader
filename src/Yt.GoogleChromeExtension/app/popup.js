// Copyright (c) 2012 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

document.addEventListener('DOMContentLoaded', function () {
    function getVideoInfo() {        
        var link = document.getElementById('videoLink').value;

        var isPlaylist = !(link.includes('https://www.youtube.com/watch?v='));
        var idResource = link.split('=')[1];

        var data = JSON.stringify({
            "idResource": idResource,
            "isPlaylist": isPlaylist
        });

        var url = 'https://localhost:44307/api/v1/yt/download'

        $('#result').val('');
        $('#result').text('');
        $('#result').hide();

        var req = new XMLHttpRequest();       
        req.open("POST", url, true);
        req.setRequestHeader("Content-Type", "application/json");
        req.onreadystatechange = function () {
            if (req.readyState === 4 && req.status === 200) {
                var result = JSON.parse(this.responseText);
                console.log(result);

                var strHtml = '';
                for (var i = 0; i < result.data.length; i++) {
                    var video = result.data[i];

                    var qualityLabels = [];
                    video.videoFormats.map((item) => {
                        qualityLabels.push({ label: item.qualityLabel, url: item.url })
                    });

                    strHtml += '<div class="card">';
                    strHtml += '     <div class="card-horizontal">';
                    strHtml += '         <div class="img-square-wrapper">';
                    strHtml += '             <img class="thumb" src="' + video.thumbnails[0].url + '" alt="' + video.title + '">';
                    strHtml += '         </div>';
                    strHtml += '         <div class="card-body">';
                    strHtml += '             <h4 class="card-title">' + video.title + '</h4>';

                    if (qualityLabels.length > 0) {
                        for (var j = 0; j < qualityLabels.length; j++) {
                            strHtml += '             <a class="label_quality" href="' + qualityLabels[j].url + '" download>' + qualityLabels[j].label + '</a>';
                            strHtml += '             <input type="hidden" data-url="' + qualityLabels[j].url + '" />'
                            strHtml += '             <input type="hidden" data-title="' + video.title + '" />'
                        }
                    }
                    strHtml += '         </div>';
                    strHtml += '     </div>';
                    strHtml += ' </div>';
                }
                $(strHtml).appendTo('#result');
                $('#result').show();
            } else {
                if (req.readyState === 4 && req.status !== 200) {
                    var error = JSON.parse(req.responseText);
                    var strHtml = '';
                    strHtml += '<div class="alert alert-yt-danger alert-dismissible fade show" role="alert">';
                    strHtml += '    ' + error.message;
                    strHtml += '    <button type="button" class="close" data-dismiss="alert" aria-label="Close">';
                    strHtml += '        <span aria-hidden="true">&times;</span>';
                    strHtml += '    </button>';
                    strHtml += '</div>';

                    $(strHtml).appendTo('#result');
                    $('#result').show();
                }

                
                // https://www.youtube.com/watch?v=dRQeQbYBMPg
            }
        }
        req.send(data);
    }

    document.getElementById('btnDownload').onclick = getVideoInfo;
});

