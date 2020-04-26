﻿function SyncControl(getNewTracksUrl, importTracksUrl) {
    $('#syncButton').click(function() {
        var audioStr = $("#inputAudioStr").val();
        var yaPlaylistId = $('#yaPlaylists').find(':selected').val();
        $.ajax({
            'type': 'GET',
            'data': 'audioStr=' + audioStr + '&' + 'yaPlaylistId=' + yaPlaylistId,
            'success': function(data) { $('#inputTracks').html(data) },
            'error': function() {},
            'url': getNewTracksUrl,
            'cache': false
        })
    });

    $('#importButton').click(function() {
        var yaPlaylistId = $('#yaPlaylists').find(':selected').val();

        var selectdTracks = $("#inputTracks").find('select').find(':selected');
        if (selectdTracks.length == 0) {
            selectdTracks = $("#inputTracks").find('select').find('option');
        }

        var tracks = [];
        for (var i = 0; i < selectdTracks.length; ++i) {
            var data = $(selectdTracks[i]).attr('value');
            tracks.push(JSON.parse(data));
        }

        var tracksForm = {
            Tracks: {
                Tracks: tracks
            },
            YaPlaylistId: yaPlaylistId
        };

        $.ajax({
            'type': 'POST',
            'data': JSON.stringify({ 'tracksForm': tracksForm }),
            'success': function() { /*progressbar*/ },
            'error': function() {},
            'url': importTracksUrl,
            'cache': false,
            'contentType': 'application/json; charset=UTF-8'
        });
    });

    $('#yaPlaylists').change(function() {
        $('#yaPlaylistForm').submit();
    });
};

var inputSuccess = false;
var yaSuccess = false;

function InputSuccess(data) {
    $('#inputTracks').html(data);
    inputSuccess = true;
    ToggleButtons();
}

function InputFail() {
    $('#inputTracks select').html('');
    inputSuccess = false;
    ToggleButtons();
}

function YaSuccess(data) {
    $('#yaTracks').html(data);
    yaSuccess = true;
    ToggleButtons();
}

function YaFail() {
    $('#yaTracks select').html('');
    $('#yaPlaylists option').html('');
    yaSuccess = false;
    ToggleButtons();
}

function UnauthorizeYa(url) {
    $.ajax({
        'type': 'POST',
        'success': function() { YaFail() },
        'error': function() { alert('Что-то пошло не так.') },
        'url': url,
        'cache': false
    });
}

function ToggleButtons() {
    var importButton = $('.importButtons');

    if (!inputSuccess || !yaSuccess) {
        importButton.attr('disabled', 'disabled');
        return;
    }

    importButton.removeAttr('disabled');
}