# README

Project thực hiện gửi MO để test **SmsDeliveredAPI** phía đối tác

Cặp khóa `test-message-delivered.key` và `test-message-delivered.public.key` được dùng với mục đích test

**PostDeliveredAPI** sẽ dùng `Private Key` `test-message-delivered.key` để mã hóa và gửi đến **SmsDeliveredAPI**

**SmsDeliveredAPI** sẽ dùng `Public Key` `test-message-delivered.public.key` để verify signature và thực hiện nhận Mo
