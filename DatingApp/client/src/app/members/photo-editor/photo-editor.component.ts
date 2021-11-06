import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { PhotoService } from 'src/app/_services/photo.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member: Member;

  uploader:FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User;
 
  constructor (private photoService: PhotoService, private accountService: AccountService,
     private toastrSerice: ToastrService ){
      this.accountService.currentUser$.pipe(take(1)).subscribe( user => this.user = user);
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      itemAlias: 'photo',
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    }

    this.uploader.onSuccessItem = (item, response, status, header) => {
      if(response){
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    }
  }

  public fileOverBase(e:any):void {
    this.hasBaseDropZoneOver = e;
  }

  removePhoto(photo: Photo){
    this.photoService.removePhoto(photo).subscribe( () => {
      const index = this.member.photos.indexOf(photo, 0);
      if (index > -1) {
        this.member.photos.splice(index, 1);
        this.toastrSerice.success("Photo has been deleted")
      }
    });
  }

  setMainPhoto(photo: Photo){
    this.photoService.setMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(x => {
        if(x.isMain = true) x.isMain = false;
        if(x.id === photo.id) x.isMain = true;
      })
    });
  }

}
