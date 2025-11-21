# Profile Picture API Documentation

This document describes the profile picture functionality added to the UserController.

## Overview

The profile picture API allows users to upload, manage, and display their profile pictures. Profile pictures are stored as JPG/JPEG files in the server's file system and the file path is stored in the User table.

## API Endpoints

### 1. Upload Profile Picture
**POST** `/api/User/UploadProfilePicture`

Uploads a new profile picture for the authenticated user.

**Headers:**
- `Authorization: Bearer {JWT_TOKEN}` (Required)
- `Content-Type: multipart/form-data`

**Request Body:**
- `file`: JPG/JPEG image file (max 5MB)

**Response:**
```json
{
  "success": true,
  "message": "Profile picture uploaded successfully.",
  "profilePictureUrl": "/uploads/profile-pictures/profile_123_20241231120000.jpg",
  "profilePicturePath": "/uploads/profile-pictures/profile_123_20241231120000.jpg"
}
```

**Error Responses:**
- `400 Bad Request`: Invalid file type, file too large, or no file uploaded
- `401 Unauthorized`: Invalid or missing JWT token
- `404 Not Found`: User not found
- `500 Internal Server Error`: Server error during upload

### 2. Remove Profile Picture
**DELETE** `/api/User/RemoveProfilePicture`

Removes the current user's profile picture.

**Headers:**
- `Authorization: Bearer {JWT_TOKEN}` (Required)

**Response:**
```json
{
  "success": true,
  "message": "Profile picture removed successfully."
}
```

**Error Responses:**
- `401 Unauthorized`: Invalid or missing JWT token
- `404 Not Found`: User not found
- `500 Internal Server Error`: Server error during removal

### 3. Get User Profile (Updated)
**GET** `/api/User/Profile`

Returns the current user's profile information including the profile picture URL.

**Headers:**
- `Authorization: Bearer {JWT_TOKEN}` (Required)

**Response:**
```json
{
  "userID": 123,
  "userName": "john_doe",
  "email": "john@example.com",
  "profilePicture": "/uploads/profile-pictures/profile_123_20241231120000.jpg",
  "googleId": null,
  "googleName": null,
  "googlePicture": null,
  "isGoogleUser": false,
  "addressID": null,
  "address": null
}
```

## File Storage

- Profile pictures are stored in: `{ProjectRoot}/uploads/profile-pictures/`
- Files are named with pattern: `profile_{userId}_{timestamp}.jpg`
- Files are served statically via: `https://your-domain.com/uploads/profile-pictures/filename.jpg`
- Maximum file size: 5MB
- Supported formats: JPG, JPEG only

## Database Schema

The `User` table now includes:
```sql
ALTER TABLE [User] ADD [ProfilePicture] nvarchar(max) NULL;
```

## Frontend Integration

### Upload Profile Picture
```javascript
const uploadProfilePicture = async (file) => {
  const formData = new FormData();
  formData.append('file', file);

  const response = await fetch('/api/User/UploadProfilePicture', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`
    },
    body: formData
  });

  const result = await response.json();
  return result;
};
```

### Display Profile Picture
```javascript
const getProfilePictureUrl = (profilePicturePath) => {
  if (!profilePicturePath) {
    return '/default-avatar.jpg'; // Default avatar
  }
  return profilePicturePath; // This will be served statically
};
```

### Remove Profile Picture
```javascript
const removeProfilePicture = async () => {
  const response = await fetch('/api/User/RemoveProfilePicture', {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });

  const result = await response.json();
  return result;
};
```

## Security Considerations

1. **File Type Validation**: Only JPG/JPEG files are allowed
2. **File Size Limit**: Maximum 5MB per file
3. **Authentication Required**: All endpoints require valid JWT token
4. **User Isolation**: Users can only modify their own profile pictures
5. **Automatic Cleanup**: Old profile pictures are automatically deleted when a new one is uploaded

## Error Handling

The API includes comprehensive error handling for:
- Invalid file types
- File size exceeded
- Authentication failures
- Database errors
- File system errors

## Migration

The database migration `20250831130005_AddProfilePictureField` adds the ProfilePicture column to the User table. Run the following command to apply:

```bash
dotnet ef database update
```
