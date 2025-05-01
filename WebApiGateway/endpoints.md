### **Endpoints agrupados:**  
#### **Usuarios:**  
- `GET /User/GetUsers`  
- `POST /User/RegisterUsers`  
- `POST /User/LogUser`  
- `GET /User/ConfirmEmail/{token}`  
- `GET /User/RequestChangePassword/{email}`  
- `PATCH /User/ChangePassword`  

#### **Comunidades:**  
- `POST /api/Community`  
- `POST /api/Community/SubscribeToCommunity/{comid}`  
- `GET, PUT, DELETE /api/Community/{id}`  
- `GET /api/Community/All`  
- `GET /api/Community/bycreator/{creatorId}`  
- `GET /api/Community/bysubscrition`  
- `DELETE /api/Community/UnsubscribeToCommunity/{comid}`  

#### **Respuestas:**  
- `POST /api/Responses/CrearRespuesta`  
- `POST /api/Responses/LikeResponse/{responseId}`  
- `GET /api/Responses/{threadId}/Responses`  
- `GET /api/Responses/{responseId}/Replies`  
- `GET /api/Responses/{idResponse}`  
- `GET /api/Responses/bycreator/{creatorId}`  
- `GET /api/Responses/byLike/{creatorId}`  
- `PUT /api/Responses/UpdateaRespuesta`  
- `DELETE /api/Responses/UnlikeResponse/{responseId}`  

#### **Roles:**  
- `GET /api/Roles/RolesComunidad/{idCommunity}`  
- `POST /api/Roles/assignRole`  
- `DELETE /api/Roles/removeRole`  

#### **Hilos:**  
- `POST /api/Thread`  
- `POST /api/Thread/LikeThread/{threadId}`  
- `GET /api/Thread/All`  
- `GET /api/Thread/AllByComunity/{idCom}`  
- `DELETE /api/Thread/UnlikeThread/{threadId}`  

#### **Mensajes:**  
- `GET, POST /api/Messages`  

#### **Follows:**  
- `GET /api/Follows/followers/{userId}`  
- `GET /api/Follows/following/{userId}`  
- `POST /api/Follows/StartFollowing`  
- `DELETE /api/Follows/StopFollowing`  
- `DELETE /api/Follows/DropFollower`  

#### **Perfil de usuario:**  
- `GET /api/UserProfile/Users`  
- `GET /api/UserProfile/Users/GetUserById/{userId}`  
- `POST /api/UserProfile/UserCreate`  
- `PATCH /api/UserProfile/UserUpdate`  

#### **Respuestas (nuevos):**  
- `GET /api/Response/GetResponseById/{id}`  
- `POST /api/Response/CreateResponse`  
- `PUT /api/Response/UpdateResponse`  
- `DELETE /api/Response/DeleteResponse/{responseId}`  

#### **Reseñas (nuevos):**  
- `GET /api/Reviews/GetReviewsByWorkshop/{workshopId}`  
- `GET /api/Reviews/GetReviewById/{id}`  
- `GET /api/Reviews/CountReviews/{workshopId}`  
- `GET /api/Reviews/GetMediaReview/{workshopId}`  
- `POST /api/Reviews`  
- `PUT /api/Reviews/ReviewUpdate`  
- `DELETE /api/Reviews/{id}`  