import test3Js from '@/common/test3'

let content = document.getElementById('content');
content.innerHTML += 'this is page2.js' + '<br />';
content.innerHTML += test3Js.msg + '<br />';