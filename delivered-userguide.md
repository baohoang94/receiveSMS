# Hướng dẫn nhận bản tin MO - Delivered Message

Dành cho đối tác có sử dụng dịch vụ nhận tin nhắn từ phía người dùng cuối, xử lý và phản hồi tin nhắn cho khách hàng

## 1.Mô tả trình tự thực hiện

1. Đối tác dựng API DeliveredMessage ở phía Server của đối tác

2. SAMI sẽ gửi public key của SAMI cho đối tác để verify sinature trong bản tin MO

3. Đối tác gửi địa chỉ API DeliveredMessage cho SAMI để cấu hình chuyển MO

## 2.Hướng dẫn chi tiết dựng API Delivered Message

Triển khai 01 endpoint nhận bản tin MO từ phía SAMI phải có 2 phương thức là GET và POST

Phương thức GET được sử dụng để SAMI check service có thông không trước khi gửi bản tin MO cho phía đối tác

Phương thức POST được sử dụng để gửi bản tin MO

Giả sử endpoint phía đối tác là `/api/delivered/messages`

### 2.1 Mô tả phương thức GET

Mục đích: Xác định API vẫn đang hoạt động để phía SAMI-S thực hiện METHOD POST

```bash
curl -X GET `/api/delivered/messages`

Response Status: 200 OK
```

| StatusCode | Description |
|-----------|-----------------------------------------------------------------------------------|
| 200 | API thông, sẵn sàng kết nối |
| Other number | API chưa sẵn sàng |

### 2.2 Mô tả phương thức POST

```bash
curl -X POST "/api/delivered/messages" -H "accept: text/plain" -H "Content-Type: application/json" -d "{\"payload\":\"string\",\"signature\":\"string\"}"
```

#### 2.2.1 Trình tự xử lý

1. Nhận json Body

2. Lấy public key của SAMI verify signature có phù hợp với payload không

3. Nếu không phù hợp trả về lỗi

4. Nếu phù hợp thực hiện các bước tiếp theo

5. Deserialize Payload thành JsonObject

6. Xử lý bản tin nhận được

7. Trả về response

#### 2.2.2 Mô tả request body

```json
{
  "$id": "https://localhost/api/references/schemas/sms-transaction-request-json-schema.json",
  "$schema": "http://json-schema.org/draft-04/schema",
  "title": "JSON Schema for message delivered Request format",
  "type": "object",
  "required": [ "payload", "signature" ],
  "properties": {
    "payload": {
      "type": "string",
      "description": "Payload of transaction sms Base64 Encode"
    },
    "signature": {
      "type": "string",
      "description": "Signature for Payload Base64 Enocde"
    }
  }
}
```

#### 2.2.3 Mô tả payload sau khi đã được giải mã

```json
{
  "$id": "https://localhost/api/references/schemas/delivered-transaction-json-schema.json",
  "$schema": "http://json-schema.org/draft-04/schema",
  "title": "JSON Schema for SmsTransaction format",
  "type": "object",
  "required": [ "transactionId", "cooperateId", "messagesDelivered", "createTime" ],
  "properties": {
    "transactionId": {
      "description": "TransactionId send mo",
      "type": "string"
    },
    "cooperateId": {
      "description": "CooperateId",
      "type": "integer"
    },
    "messagesDelivered": {
      "description": "An array of delivered.",
      "items": { "$ref": "#/definitions/messageDelivered" },
      "minItems": 1,
      "type": "array"
    },
    "createTime": {
      "description": "Event sending time",
      "format": "date-time",
      "type": "string"
    }
  },
  "definitions": {
    "messageDelivered": {
      "type": "object",
      "required": [ "smsInGuid", "subscriber", "message", "shortCode", "receivedTime", "operatorId" ],
      "properties": {
        "smsInGuid": {
          "type": "string",
          "description": "Mã định danh tin nhắn gửi từ phía đối tác"
        },
        "subscriber": {
          "type": "string",
          "description": "Số điện thoại gửi"
        },
        "message": {
          "type": "string",
          "description": "Thông điệp khách hàng gửi tin nhắn"
        },
        "shortCode": {
          "type": "string",
          "description": "Đầu số khách hàng gửi tin"
        },
        "receivedTime": {
          "type": "string",
          "description": "Thời gian nhận message delivered"
        },
        "operatorId": {
          "type": "number",
          "enum": [ 0, 1, 2, 3, 4, 5, 6 ],
          "description": "Subscriber thuộc nhà mạng nào 0: mobifone, 1: vinaphone, 2: viettel, 3: vietnamobile, 4: gtel"
        }
      }
    }
  }
}
```

#### 2.2.4 Response trả về cho SAMI-S

##### Một request thành công, api sẽ trả về nội dung Response như sau

HTTP Status Code: 200 OK

Json Response Schema

```json
{
  "$schema": "https://localhost/api/references/schemas/delivered-response-json-schema.json",
  "title": "JSON Schema for delivered response format",
  "type": "object",
  "properties": {
    "status": {
      "type": "number",
      "description": "Trạng thái của response"
    },
    "message": {
      "type": "string",
      "description": "Mô tả cho status"
    },
    "data": {
      "transactionID": {
        "type": "string",
        "description": "transactionId nhận được từ SAMI-S (Có trong bản tin được giải mã)"
      },
      "deliveredReports": {
        "description": "An array of response data.",
        "items": { "$ref": "#/definitions/deliveredReport" },
        "type": "array"
      }
    },
    "responseTime": {
      "type": "string",
      "format": "date-time",
      "description": "Thời gian trả response"
    }
  },
  "definitions": {
    "deliveredReport": {
      "type": "object",
      "required": [ "statusCode", "statusMessage", "smsInGuid", "responseId"],
      "properties": {
        "smsInGuid": {
          "type": "string",
          "description": "Mã định danh tin nhắn gửi từ phía SAMI-S"
        },
        "statusMessage": {
          "type": "string",
          "enum": [ "RECEIVED", "DUPLICATE" ],
          "description": "Mô tả trạng thái nhận từ phía đối tác"
        },
        "responseId": {
          "type": "string",
          "description": "Id ghi nhận từ phía đối tác"
        }
      }
    }
  }
}
```

##### Mô tả

| status | Status Description |
|-----------|-----------------------------------------------------------------------------------|
| >=0 | Thành công, SAMI sẽ không gửi lại |
| < 0 | Không thành công, SAMI sẽ retry lại bản tin MO |

Mã lỗi trả về trong report

| statusCode | StatusCode Description |
|-----------|-----------------------------------------------------------------------------------|
| 0 | Nhận thành công - RECEIVED |
| 2 | Bản tin đã tồn tại - DUPLICATE |

##### Một request không thành công, api sẽ trả Http StatusCode không phải trạng thái 200

#### Những điểm lưu ý

1. Nếu mã hóa không đúng, IP không được phép gửi, hoặc gửi Short Code không được cung cấp, hệ thống sẽ banned IP trong vòng 20 phút

2. Chỉ dùng JSON để trao đổi

### 2.3 Ví dụ tham khảo

Tham khảo Project **SmsDeliveredAPI** viết bằng Csharp .NET Core 3.1

Thực hiện kiểm tra nội bộ bằng cách sinh cặp khóa bằng lệnh

Ví dụ sau tạo `Private Key` sử dụng openssl

```bash
openssl genrsa -out test-message-delivered.key 2048
```

Sinh `Public Key` từ `Private Key`

```bash
openssl rsa -in test-message-delivered.key -pubout -out test-message-delivered.public.key
```

Cài đặt db, sinh bảng theo script trong thư mục `./Data/database_script.sql`

Deployment **SmsDeliveredAPI**

Copy `test-message-delivered.public.key` vào thư mục PublicKey của **SmsDeliveredAPI**

Copy `test-message-delivered.key` vào thư mục PrivateKey của **PostDeliveredAPI**

Điều chỉnh đường dẫn API trong **PostDeliveredAPI** để thực hiện test

> Chú ý triển khai
>
> Sau khi thực hiện test nội bộ xong, khi triển khai thực tế sẽ update public key của SAMI-S thay thế cho (test-message-delivered.public.key)
>
> Thêm địa chỉ IP của SAMI-S là `115.84.179.221` vào trường `IPClientSafeList` trong file `appsettings.json` để SAMI-S có thể gọi được API từ phía đối tác
