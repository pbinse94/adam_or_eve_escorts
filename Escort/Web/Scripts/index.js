
import {
    ChatRoom,
    DeleteMessageRequest,
    DisconnectUserRequest,
    SendMessageRequest,
} from '../node_modules/amazon-ivs-chat-messaging/dist/cjs/index'



window.MakeRoom = function MakeRoom(regionOrUrl, tokenProvider) {
    
    return new ChatRoom(regionOrUrl, tokenProvider);
}


window.SendMessageRequestLocal = function SendMessageRequestLocal(content) {
    return new SendMessageRequest(content);
}