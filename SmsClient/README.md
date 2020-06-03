# README

Project thực hiện gửi MT tới API của SAMI-S

Private Key `your.company.key` trong thư mục `./PrivateKey/` hiện tại được dùng để test (có thể `your.company.key` đã được SAMI-S đổi tên thành key có tên là tên cty của bạn trong thư mục `./PrivateKey/` để hướng dẫn thực hiện test kết nối)

Khi triển khai thực tế, đối tác sinh lại cặp khóa bất đối xứng. Thay `Private Key` `your.company.key` test bằng `Private Key` mới sinh. Sau đó gửi `Public Key` cho SAMI-S

Ví dụ sau tạo `Private Key` sử dụng openssl

```bash
openssl genrsa -out your.company.key 2048
```

Sinh `Public Key` từ `Private Key` để gửi cho SAMI-S

```bash
openssl rsa -in your.company.key -pubout -out your.company.public.key
```
