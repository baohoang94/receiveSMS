# Hướng dẫn gửi bản tin MT - Submit Message

## 1.Mô tả trình tự thực hiện

1. Đối tác tạo cặp khóa bất đối xứng bằng thuật toán RSA, gửi `Public Key` cho SAMI để SAMI import vào hệ thống

    Ví dụ sau tạo `Private Key` sử dụng openssl

    ```bash
    openssl genrsa -out your.company.key 2048
    ```

    Sinh `Public Key` từ `Private Key` để gửi cho SAMI-S

    ```bash
    openssl rsa -in your.company.key -pubout -out your.company.public.key
    ```

2. SAMI sẽ import `Public Key` của đối tác vào hệ thống và cấp tài khoản truy cập API

3. Đối tác dùng `Private Key` để ký vào transaction và gửi qua API

4. SAMI sẽ sử dụng khóa `Public Key` của đối tác verify dữ liệu sau đó thực hiện các thủ tục gửi tin nhắn

Những điểm lưu ý với các tham số:

1. `TransactionID` luôn luôn duy nhất mỗi lần gọi, hệ thống sẽ dùng `TransactionID` để chống lặp transaction

2. `cooperateMsgId` luôn luôn duy nhất mỗi lần gọi, hệ thống sẽ dùng `cooperateMsgId` để chống lặp tin

3. Hệ thống nhận được Transaction Request sẽ so sánh thời gian gửi `CreateTime` với thời gian hệ thống, nếu quá 5 phút, hệ thống sẽ không cho phép gửi Transaction này

Bảng mô tả các quy tắc gửi SMS

| Value | Description |
|-----------|-----------------------------------------------------------------------------------|
| 100 | Số lần gọi sai cho phép, nếu lớn hơn giá trị này sẽ bị chặn |
| 5 | Thời gian cho phép gọi service sau khi bị chặn SPAM (tính bằng phút) |
| 300 | Số lượng tin nhắn được phép gửi trong 1 ngày tới 1 thuê bao |
| 150 | Số lượng tin nhắn được phép gửi trong 1 giờ tới 1 thuê bao |
| 25 | Số lượng tin nhắn được phép gửi trong 1 phút tới 1 thuê bao |
| 1000 | Số lượng tin nhắn được phép gửi trong 1 tháng tới 1 thuê bao |
| 1000 | Độ dài tin nhắn tối đa được phép (tính bằng ký tự) |
| 300 | Thời gian trễ được phép của Transaction so với hệ thống (tính bằng giây) |


Mã lỗi trả về trong report

| ErrorCode | ErrorDescription |
|-----------|-----------------------------------------------------------------------------------|
| 0 | Tin nhắn hợp lệ |
| 1 | CooperateMsgId đã tồn tại. |
| 2 | Gửi tin tới 1 số thuê bao vi phạm luật Spam (tra cứu bảng trên) |
| 3 | Không xác định được nhà mạng (TELCO) cho số điện thoại |
| 4 | Đối tác không được cấp quyền sử dụng ShortCode |
| 5 | Số điện thoại khách hàng nằm trong danh sách từ chối dịch vụ |
| 6 | Đối tác không được gửi MT chủ động khi không có MO trên ShortCode |
| 7 | Đối tác gửi Message trên ShortCode không giống mẫu đăng ký (Áp dụng trong trường hợp phải tuân theo mẫu đăng ký) |
| 8 | Đối tác đang ở chế độ thử nghiệm, chỉ được gửi tới Số điện thoại trong danh sách đăng ký. |

## 2.Hướng dẫn chi tiết SendSMS và nhận report

### 2.1 Gửi nhiều tin với nội dung khác nhau

#### Mô tả SMS TRANSACTION JSON OBJECT

```json
{
  "$id": "https://sms.sami.vn:8558/api/references/schemas/sms-transaction-json-schema.json",
  "$schema": "http://json-schema.org/draft-04/schema",
  "title": "JSON Schema for SmsTransaction format",
  "type": "object",
  "required": [ "transactionId", "coopereateId", "smsOuts", "createTime" ],
  "properties": {
    "transactionId": {
      "description": "TransactionId send sms",
      "type": "string"
    },
    "coopereateId": {
      "description": "CooperateId",
      "type": "integer"
    },
    "smsOuts": {
      "description": "An array of sms.",
      "items": { "$ref": "#/definitions/smsOutItem" },
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
    "smsOutItem": {
      "type": "object",
      "required": [ "cooperateMsgId", "destAddr", "message", "shortCode", "cdrIndicator", "mtType" ],
      "properties": {
        "cooperateMsgId": {
          "type": "string",
          "description": "Mã định danh tin nhắn gửi từ phía đối tác"
        },
        "destAddr": {
          "type": "string",
          "description": "Số điện thoại gửi"
        },
        "message": {
          "type": "string",
          "description": "Thông điệp gửi cho DesAddr"
        },
        "shortCode": {
          "type": "string",
          "description": "Brandname hoặc đầu số gửi tin"
        },
        "cdrIndicator": {
          "type": "string",
          "description": "Có tính cước không"
        },
        "mtType": {
          "type": "string",
          "enum": [ "AD", "AN", "SI" ],
          "description": "Loại tin nhắn gửi"
        },
        "smsInGuid": {
          "type": [ "string", "null" ],
          "description": "Id của tin MO (nếu có)"
        },
        "operatorId": {
          "type": [ "number", "null" ],
          "description": "Nhà mạng"
        }
      }
    }
  }
}
```

#### Mô tả TransactionRequest

```json
{
    "$id": "https://sms.sami.vn:8558/api/references/schemas/sms-transaction-request-json-schema.json",
    "$schema": "http://json-schema.org/draft-04/schema",
    "title": "JSON Schema for SmsTransaction Request format",
    "type": "object",
    "required": [ "Payload", "Signature"],
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

#### Các bước thực hiện

##### 1. Tạo SMS TRANSACTION JSON OBJECT `jsonTran`

```json
{
    "transactionId": "uniqueID",
    "coopereateId" : 1,
    "smsOuts" : [
        {
            "cooperateMsgId": "Message Id of Cooperate",
            "destAddr" : "0912xxxxxx",
            "message" : "Message want to send to DestAddr",
            "shortCode": "Brandname or ShortCode",
            "mtType" : "AN",
            "createTime": "2019-10-10T11:37:27.328714+07:00"
        }
        // Các bản tin tiếp theo ...
    ]
}
```

##### 2. Chuyển đổi sang mảng Bytes Array với encoding là UTF-8

##### 3. Dùng khóa Private ký vào `byteData` được `byteSignature`

##### 4. Đóng gói `byteData` thành Base64 và đưa vào `payload`

##### 5. Đóng gói `byteSignature` thành Base64 và đưa vào `signature`

##### 6. Tạo JSON Transaction Request

```json
{
    "payload": "Base64 Encoding",
    "signature": "Base64 Encoding",
}
```

##### 7. Response trả về

###### Một request thành công, api sẽ trả về nội dung Response như sau

HTTP Status Code: 200 OK

Json Body

```json
{
    "transactionId": "uniqueID",
    "responseTime" : "2019-10-10T13:23:19.3949519+07:00",
    "smsReports" : [
        {
            "responeId": "346d9fc6-10d3-4561-b19a-f4ec1776f2ae",
            "cooperateMsgId": "1",
            "statusCode": 0,
            "statusMessage": null,
            "receivedTime": "2019-10-10T13:23:19.3949519+07:00"
        }
        // Các bản tin tiếp theo ...
    ]
}
```

###### Một request không thành công, api sẽ trả nội dung Response như sau:

HTTP Status: 4xx hoặc 5xx

Json Body

```json
{
    "ErrorCode": errorCode,
    "ErrorMessage": "Mô tả lỗi"
}
```

#### Những điểm lưu ý

1. Nếu mã hóa không đúng, IP không được phép gửi, hoặc gửi Short Code không được cung cấp, hệ thống sẽ banned IP trong vòng 20 phút

2. Chỉ dùng JSON để trao đổi

3. Gửi MT qua đầu số ngắn (Ví dụ 8050) mà không phải là BrandName thì bắt buộc phải có smsInGuid đi kèm để xác định bản tin MT trả lời cho MO nào. Không được gửi MT chủ động sử dụng ShortCode là đầu số ngắn.

4. Đối với BrandName trên mạng Mobifone, bắt buộc phải đăng ký template gửi tin. Không thể gửi tin nhắn có nội dung ngoài template

### 2.2 Get Report

Request:

Method GET:

`api/sms/report/{id}`

Json Schema

```json
{
  "$id": "https://sms.sami.vn:8558/api/references/schemas/sms-report-json-schema.json",
  "$schema": "http://json-schema.org/draft-04/schema",
  "title": "JSON Schema for status of MT format",
  "type": "object",
  "required": [ "smsOutGuid", "cooperateMsgId", "status", "sentTime", "reportTime" ],
  "properties": {
    "reportId": {
      "type": "number",
      "description": "Id của report"
    },
    "cooperateId": {
      "type": "number",
      "description": "Id của đối tác gửi bản tin MT"
    },
    "cooperateMsgId": {
      "type": "string",
      "description": "Id của đối tác khi gửi MT sang SAMI"
    },
    "status": {
      "type": "string",
      "enum": [ "SENT", "SENTGATEWAY", "FAIL", "PENDING" ],
      "description": "Trạng thái bản tin MT"
    },
    "sentTime": {
      "type": "string",
      "format": "date-time",
      "description": "Thời gian gửi bản tin MT"
    },
    "reportTime": {
      "type": "string",
      "format": "date-time",
      "description": "Thời gian nhận được report"
    },
    "smsOutGuid": {
      "type": "string",
      "description": "Id của SAMI khi ghi nhận MT từ đối tác gửi sang"
    },
    "smsInGuid": {
      "type": [ "string", "null" ],
      "description": "Id Mo trong trường hợp MT gửi reply cho 1 MO cụ thể"
    },
    "sentIp": {
      "type": "string",
      "description": "Địa chỉ client đã gửi bản tin MT"
    },
    "sentMethod": {
      "type": "string",
      "description": "Phương thức client đã gửi bản tin MT"
    }
  }
}
```

Ví dụ kết quả trả về

```json
{
  "reportId": 5446437,
  "cooperateId": 46045,
  "cooperateMsgId": "5844b1ee-6bfb-45bd-a680-e71f054803c2",
  "status": "SENT",
  "sentTime": "2020-05-20T00:13:23.54",
  "reportTime": "2020-05-20T00:13:36.433",
  "smsOutGuid": "9c906be2-0e92-421d-b76d-e509deb6bb57",
  "smsInGuid": null,
  "sentIp": "117.4.246.169",
  "sentMethod": "api/sms/send"
}
```

### 2.3 Tham khảo

- Tham khảo Project **SmsClient** viết bằng Cshap, framework .NET Core 3.1
- Tham khảo API gửi sms có thể xem [https://sms.sami.vn:8558/swagger/index.html](https://sms.sami.vn:8558/swagger/index.html)
- Tham khảo API lấy token có thể xem [https://auth.sami.vn:8443/swagger/index.html](https://auth.sami.vn:8443/swagger/index.html)

> Ghi chú: Để test và xem được API đối tác gửi IP từ đó gọi SmsClient để SAMI-S kích hoạt địa chỉ
