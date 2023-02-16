$('.concedeCheckboxes').click(function () {
    let player1concede = $('#Player1Concede').prop('checked');
    let player2concede = $('#Player2Concede').prop('checked');

    if (!(player1concede && player2concede)) {
        return;
    }
    else {
        var name = $(this).prop('name');

        if (name === 'Player1Concede') {
            $('#Player2Concede').prop('checked', false);
        }
        else {
            $('#Player1Concede').prop('checked', false);
        }
    };
});