$(function () {
    disableInput();
    $('#username').removeAttr('disabled');

    // player id
    let playerId;

    // game hub object
    let gameHub = $.connection.gameHub;

    // The username is already taken
    gameHub.client.usernameTaken = function () {
        $('#status').html("The username is already taken.");
        $('#usernameGroup').addClass("has-error");
    };

    // Starts a new game by displaying the board and showing whose turn it is
    gameHub.client.start = function (game) {
        buildBoard(game.Board);
        let opponent = getOpponent(game);
        displayOpponent(opponent);
        displayTurn(game.WhoseTurn, true);
    };

    // A piece has been placed on the board
    gameHub.client.piecePlaced = function (row, col, piece) {
        $('#pos-' + row + '-' + col).html(piece);
    };

    // display player turn
    gameHub.client.updateTurn = function (game) {
        displayTurn(game.WhoseTurn);
    };

    // Tie
    gameHub.client.tieGame = function () {
        $('#status').html("It is a Tie!!");
        $('td[id^=pos-]').off('click');
        enableInput();
    };

    // Winner
    gameHub.client.winner = function (playerName) {
        $('#status').html("Winner is " + playerName);
        $('td[id^=pos-]').off('click');
        enableInput();
    };

    // respond server
    $('#findGame').click(function () {
        var chosenUsername = $('#username').val();
        gameHub.server.findGame(chosenUsername);
    });

    // enable input username & button
    function enableInput() {
        $('#username').removeAttr('disabled');
        $('#findGame').removeAttr('disabled');
        $('#username').focus();
    };

    // disable input
    function disableInput() {
        $('#username').attr('disabled', 'disabled');
        $('#findGame').attr('disabled', 'disabled');
    };

    // if piece filled, click not allowed
    function endGame() {
        // Remove click handlers from board positions
        $('td[id^=pos-]').off('click');
        enableInput();
    };

    // Turn display
    function displayTurn(playersTurn, isDisplayingOpponent) {
        let turnMessage = "";
        if (playerId == playersTurn.Id) {
            turnMessage = "Your turn";
        } else {
            turnMessage = playersTurn.Name + "\'s turn";
        }

        // Do not overwrite opponent's name if it is being displayed
        if (isDisplayingOpponent) {
            $('#status').html($('#status').html() + turnMessage);
        } else {
            $('#status').html(turnMessage);
        }
    };

    // Displays the opponents name
    function displayOpponent(opponent) {
        $('#status').html("You are playing against " + opponent.Name + "<br />");
    };

    // Build and display the board
    function buildBoard(board) {
        var template = Handlebars.compile($('#board-template').html());
        $('#board').html(template(board));

        // click handler each position
        $('td[id^=pos-]').click(function (e) {
            e.preventDefault();
            var id = this.id; // "pos-0-0"
            var parts = id.split("-"); // [pos, 0, 0]
            var row = parts[1];
            var col = parts[2];
            gameHub.server.placePiece(row, col);
        });
    };

    // Retrieves the opponent player from the game
    function getOpponent(game) {
        if (playerId == game.Player1.Id) {
            return game.Player2;
        } else {
            return game.Player1;
        }
    };

    // Connection to hub
    $.connection.hub.start().done(function () {
        enableInput();
    });
});