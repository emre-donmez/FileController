# File Controller API

This project provides an ASP.NET Core Web API for file upload, creation, and retrieval operations. The API enables clients to manage and store files under specific user identities.

## List Files and Folders

**Endpoint:** `GET /api/File/{token}` or `GET /api/File/{token}/{path}`

Lists files and folders under the specified user identity.

**Example Request:**
`/api/File/a33bdcb8-6c32-4f77-a7ab-722e3b2d5420/images`

**Example Response:**
```json
200 OK
{
  "files": [
    "50659581-4473-4520-8978-42636afa6bf8.png",
    "9a088555-f012-4a33-973c-c32677f69290.png",
    "da15ac41-07c4-483d-98bc-21a606b27403.png"
  ],
  "directories": [
    "slider",
    "logo"
  ]
}
```

### Get File

**Endpoint:** `GET /api/File/{token}/{path}/{id}`

 Retrieves a specific file under the specified user identity and path.

**Success Status:** 200 OK
- Returns the content of the requested file after a successful file retrieval operation.


## File Creation Procedures

### 1. Generating Token

To generate a token, follow these steps:

**Endpoint:** `POST /api/File/CreateToken`

This endpoint creates a new user identity (token) and associates a folder with this token.

**Example Response:**
```json
200 OK
{
    "token": "a33bdcb8-6c32-4f77-a7ab-722e3b2d5420"
}
```

### 2. Upload File

**Endpoint:** `POST /api/File/UploadFile`

This endpoint uploads a file to the specified path.

**Input Parameters:**
- `Token`: User identity where files will be uploaded.
- `File`: File to be uploaded.

**Success Status:** 200 OK
- Returns a response containing details of the files created after a successful upload operation.

**Example Request:**
```
POST /api/File/UploadFile
Content-Type: multipart/form-data

token: a33bdcb8-6c32-4f77-a7ab-722e3b2d5420
file: [Binary Data]
```

**Example Response:**
```json
200 OK
{
    "token": "a33bdcb8-6c32-4f77-a7ab-722e3b2d5420",
    "files": [
        {
            "id": "1ed6ec5e-5e5c-4c09-94ab-28f6a96417ae",
            "createdFileName": "example1.png",
            "path": "images"
        }
    ]    
}
```

### Upload Multiple Files

**Endpoint:** `POST /api/File/UploadFiles`

This endpoint uploads multiple files to the specified path.

**Input Parameters:**
- `Token`: User identity where files will be uploaded.
- `Files`: List of files to be uploaded.

**Success Status:** 200 OK
- Returns a response containing details of the files created after a successful upload operation.
