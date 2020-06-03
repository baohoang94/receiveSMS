# README

Đây là thư mục chứa Public Key của SAMI-S

`test-message-delivered.public.key` chỉ dùng với mục đích test nội bộ **SmsDeliveredAPI** phía đối tác

Trong triển khai thực tế sẽ add Public Key của SAMI-S vào thư mục này. Sau đó chỉnh cấu hình trong file `appsettings.json`

```json
{
	"PublicKey": {
		"SAMI": ".\\PublicKey\\test-message-delivered.public.key"
	}
}
```