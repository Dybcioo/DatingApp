import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root'
})
export class PhotoService {
  baseUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) { }
  
  removePhoto(photo: Photo){
    const temp = photo.url.split('/');
    return this.http.delete(this.baseUrl + "photos/" + temp[temp.length - 1]);
  }

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl + "users/set-main-photo/" + photoId, null);
  }
}
