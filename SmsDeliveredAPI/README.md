# README

Project thực hiện gửi triển khai API nhận MO phía đối tác

Cặp khóa `test-message-delivered.key` và `test-message-delivered.public.key` được dùng với mục đích test **SmsDeliveredAPI**

**PostDeliveredAPI** sẽ dùng Private Key `test-message-delivered.key` để mã hóa và gửi MO tới **SmsDeliveredAPI**

**SmsDeliveredAPI** sẽ dùng Public Key `test-message-delivered.public.key` để verify signature và thực hiện nhận Mo

Khi triển khai thực tế, bạn phải thay Public Key `test-message-delivered.public.key` bằng Public Key của SAMI.

Sau đó chỉnh cấu hình trong file `appsettings.json`

```json
{
    ...
    "PublicKey": {
    "SAMI": ".\\PublicKey\\test-message-delivered.public.key"
  }
}
```
