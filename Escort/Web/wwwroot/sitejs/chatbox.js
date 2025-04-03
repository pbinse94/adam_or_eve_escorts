
function sendCredit(index, userId, giftMessage)
{
    $.ajax({
        type: "GET",
        url: "/chatauth/sendgift",
        data: { giftindex: index, escortId: userId },
        success: function (data)
        {
            if (data.data > 0)
            {
                sendMessage(giftMessage, false);

                const creditValue = parseInt($('#creditBalance').text() || 0);
                let spent = parseInt(index);
                if (index == 0)
                {
                    spent = 1;
                }
                //else
                //{
                //    toastr.success("Gift Sent Successfully.");
                //}
                $('#creditBalance').text(creditValue - spent);
            } else
            {
                toastr.error("Don't have sufficient credits.");
            }

        }
    });
}

function getEscortCredit()
{
    $.ajax({
        type: "GET",
        url: "/streaming/GetEscortScore",
        dataType: 'json',
        success: function (response)
        {
            if (response.score > 0)
            {
                $('#creditBalance').text(response.score);
            }

        }
    });
}

const renderMessage = (message) =>
{
    
    // let messageHtml = "<p>" + $(this).val('') + "</p>";
    let messageHtml = '';
    switch (message.type) {
            case 'ERROR':
    messageHtml = `<div class='error-line'><p>${message.message}</p></div>`;
    break;
    case 'SUCCESS':
    messageHtml = `<div class='success-line'><p>${message.message}</p></div>`;
    break;
    case 'STICKER':
    messageHtml = `<div class='chat-line chat-line--sticker'><img class='chat-line-img' src='${message.avatar}' alt='Avatar for ${message.username}' /><p><span class='username'>${message.username}</span></p><img class='chat-sticker' src='${message.sticker}' alt='sticker' /></div>`;
    break;
        case 'MESSAGE':

            messageHtml = ` <div class="chatLeftBlog"><div class='chatProfilePic'><span>${message.username[0]}</span></div><div class='chatUserName'> <h6>${message.username}</h6><p>${message.message}</p></div></div>`;
    break;
        }
    $('.chatBlog').append(messageHtml);
    $('.chatBlog').animate({
        scrollTop: $('.chatBlog').prop("scrollHeight")
    }, 500);
};

function SendTrigger() {
    
    var e = $.Event('keydown', { key: 'Enter', keyCode: 13, which: 13 });
    $('#chatInput').trigger(e);
}
    $(document).ready(function () {
            

        $($('.escortsBlog1')).each(function (index, element)
    {
        const escortImageElement = $(element).find('.escortImg1');

        if ($(escortImageElement).data('src') != "")
        {
            common.getFile(`user/thumbnail_profile/${$(escortImageElement).data('src')}`, $(escortImageElement))

        }
    });
    
        handleSignIn(username, moderator, avatarUrl);

        $('#chatInput').on('keydown', function (e)
        {
            if (e.key === 'Enter' && $(this).val())
            {
                sendMessage($(this).val());
                $(this).val('');
            }
        });

        $('#stickerPickerBtn').on('click', function ()
        {
            // Implement Sticker Picker
        });

        $('#raiseHandBtn').on('click', function ()
        {
            handleRaiseHandSend();
        });
    });

    let avatarUrl = {
        src: "https://d39ii5l128t5ul.cloudfront.net/assets/chat/v1/sticker-1.png",
        name: "heart"
    }
    let username = currentuser;
    let moderator = false;
    let chatRoom = null;
    let flag = 0;
    let handRaised = false;
    let chatConfig = {
        CHAT_REGION: 'us-east-1',
        Chat_Room_Id: roomId
    }
        const uuidv4 = () =>
        {
            // eslint-disable-next-line
            return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(
                /[xy]/g,
                function (c)
                {
                    // eslint-disable-next-line
                    var r = (Math.random() * 16) | 0,
                        v = c === "x" ? r : (r & 0x3) | 0x8;
                    return v.toString(16);
                }
            );
        };

        const tokenProvider = async (selectedUsername, isModerator, avatarUrl) =>
        {

            const uuid = uuidv4();
            const permissions = isModerator ? ['SEND_MESSAGE', 'DELETE_MESSAGE', 'DISCONNECT_USER'] : ['SEND_MESSAGE'];

            const request = {
                roomidentifier: chatConfig.Chat_Room_Id,
                userId: `${selectedUsername}.${uuid}`,
                attributes: {
                    username: `${selectedUsername}`,
                    avatar: `${avatarUrl.src}`,
                },
                capabilities: permissions,
            };

            try
            {


                const response = await $.post(`/chatauth/getchattoken`, request);
                return {
                    token: response.token,
                    sessionExpirationTime: new Date(response.sessionExpirationTime),
                    tokenExpirationTime: new Date(response.tokenExpirationTime),
                };
            } catch (error)
            {
                if (flag == 0)
                {
                    swal({
                        title: 'This live stream has ended',
                        text: 'Explore our other content in the meantime!',
                        type: 'error',
                        showCancelButton: false,
                        confirmButtonColor: '#EC881D',
                        confirmButtonText: 'Go to Home'
                    }).then(function ()
                    {
                        window.location.href = '/home/index'; // Change the URL to your home page URL
                    });
                    flag++;
                }

                console.error('Error:', error);
            }
        };

        const handleSignIn = async (selectedUsername, isModerator, avatarUrl) =>
        {
            username = selectedUsername;
            moderator = isModerator;

            chatRoom = new MakeRoom({
                regionOrUrl: chatConfig.CHAT_REGION,
                tokenProvider: () => tokenProvider(selectedUsername, isModerator, avatarUrl),
            });

            chatRoom.connect();

            chatRoom.addListener('connect', () =>
            {
                renderMessage({ type: 'SUCCESS', message: 'Connected...' });
            });

            chatRoom.addListener('disconnect', (reason) =>
            {
                renderMessage({ type: 'ERROR', message: `Connection closed. Reason: ${reason}` });
            });

            chatRoom.addListener('message', (message) =>
            {
                handleMessage(message);
            });

            $('#signInModal').hide();
        };

        const giftsend=(element, index, userId, iconName) =>
        {
            const giftName = $(element).data('popup');
            const giftMessage = `just sent a <img src='/assets/images/gifts/${iconName.toLowerCase().replace("n", "") }.png' width='20px' title='${giftName}' alt='${giftName}'>`;
            //sendMessage(giftMessage, false);
            sendCredit(index, userId, giftMessage);
        }


        const handleMessage = (data) =>
        {
            const message = {
                type: data.attributes?.message_type || 'MESSAGE',
                timestamp: data.sendTime,
                username: data.sender.attributes.username,
                userId: data.sender.userId,
                avatar: data.sender.attributes.avatar,
                message: data.content,
                messageId: data.id,
            };

            if (typeof (escortId) == 'undefined')
            {
                getEscortCredit();
            }

            if (message.type === 'RAISE_HAND')
            {
                handleRaiseHand(message);
            } else if (message.type === 'STICKER')
            {
                handleSticker(message);
            } else
            {
                renderMessage(message);
            }
        };

        const sendMessage = async (message, isCreditToEscort = true) =>
        {

            const content = `${message.replace(/\\/g, '\\\\').replace(/"/g, '\\"')}`;
            const request = new SendMessageRequestLocal(content);
            try
            {
                let message = await chatRoom.sendMessage(request);
            
                if (typeof (escortId) != 'undefined' && isCreditToEscort)
                {
                    //sendCredit(0, escortId);
                }

            } catch (error)
            {
                renderMessage({ type: 'ERROR', message: `Error sending message: ${error.message}` });
            }
        };

    

        const handleRaiseHandSend = async () =>
        {
        
            const attributes = { message_type: 'RAISE_HAND' };
            const request = new SendMessageRequestLocal(`[raise hand event]`, attributes);
            try
            {

                await chatRoom.sendMessage(request);
                handRaised = !handRaised;
            } catch (error)
            {
                renderMessage({ type: 'ERROR', message: `Error raising hand: ${error.message}` });
            }
        };

        const handleRaiseHand = (message) =>
        {
            // Implement Raise Hand Logic
        };

        const handleSticker = (message) =>
        {
            // Implement Sticker Handling Logic
        };




//sticker js
let $stickerButton = $('.input-line-btn');
let $stickersContainer = $('.stickers-container');

$stickerButton.on('click', function ()
{
    $stickersContainer.toggleClass('hidden');
});

function handleStickerSend(sticker)
{
    // Implement the logic for handling the sticker send
    console.log('Sticker sent:', sticker);
}
const STICKERS = [
    { name: 'smile', src: 'https://d39ii5l128t5ul.cloudfront.net/assets/chat/v1/sticker-1.png' },
    { name: 'heart', src: 'https://d39ii5l128t5ul.cloudfront.net/assets/chat/v1/sticker-1.png' },
    // Add more stickers as needed
];

STICKERS.forEach(sticker =>
{
    const $stickerBtn = $(`
    <button class="sticker-btn" aria-label="${sticker.name}">
        <img class="sticker-item" src="${sticker.src}" alt="${sticker.name} sticker">
    </button>
    `);

    $stickerBtn.on('click', function ()
    {
        $stickersContainer.addClass('hidden');
        handleStickerSend(sticker);
    });

    $stickersContainer.append($stickerBtn);
});


// Avtar js

const AVATARS = [
    { name: 'avatar1', src: 'path/to/avatar1.png' },
    { name: 'avatar2', src: 'path/to/avatar2.png' },
    // Add more avatars as needed
];

let currentAvatar = null;

function handleAvatarClick(avatar)
{
    currentAvatar = avatar.name;
    // renderAvatars();
    console.log('Selected avatar:', avatar);
}

// function renderAvatars() {
//     const avatarsContainer = document.getElementById('avatars-container');
//     avatarsContainer.innerHTML = '';

//     AVATARS.forEach(avatar => {
//         const selected = avatar.name === currentAvatar ? ' selected' : '';

//         const button = document.createElement('button');
//         button.className = `item-container item-container--square-items${selected}`;
//         button.onclick = (e) => {
//             e.preventDefault();
//             handleAvatarClick(avatar);
//         };
//         button.onkeydown = (e) => {
//             if (e.key === 'Enter') {
//                 e.preventDefault();
//                 handleAvatarClick(avatar);
//             }
//         };

//         const img = document.createElement('img');
//         img.className = `item item--avatar${selected}`;
//         img.src = avatar.src;
//         img.alt = avatar.name;
//         img.onclick = (e) => {
//             e.preventDefault();
//             handleAvatarClick(avatar);
//         };

//         button.appendChild(img);

//         if (selected) {
//             const selectedWrapper = document.createElement('div');
//             selectedWrapper.className = 'item-selected-wrapper';

//             const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
//             svg.className = 'icon icon--selected';
//             svg.setAttribute('xmlns', 'http://www.w3.org/2000/svg');
//             svg.setAttribute('width', '24');
//             svg.setAttribute('height', '24');
//             svg.setAttribute('fill', 'white');
//             svg.setAttribute('viewBox', '0 0 24 24');

//             const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
//             path.setAttribute('d', 'M9 16.67L4.83 12.5L3.41 13.91L9 19.5L21 7.49997L19.59 6.08997L9 16.67Z');

//             svg.appendChild(path);
//             selectedWrapper.appendChild(svg);
//             button.appendChild(selectedWrapper);
//         }

//         avatarsContainer.appendChild(button);
//     });
// }

// Initial render
// renderAvatars();
