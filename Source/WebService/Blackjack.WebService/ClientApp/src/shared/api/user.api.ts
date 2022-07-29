import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserDto } from '../dtos/user.dto';

@Injectable({
  providedIn: 'root',
})
export class UserApi {

  constructor(private _http: HttpClient) {

  }

  /**
   * Retrieves a user from the server by username
   */
  public async getUserByID(userID: number): Promise<UserDto> {
    return await this._http.get<UserDto>(`User/GetUserByID?userID=${userID}`).toPromise();
  }

  public async updateUser(user: UserDto): Promise<UserDto> {
    return await this._http.post<UserDto>('User/UpdateUser', user).toPromise();
  }
}
