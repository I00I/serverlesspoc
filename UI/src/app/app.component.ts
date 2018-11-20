import { Component } from '@angular/core';
import { from, Observable } from 'rxjs';
import { combineAll, map } from 'rxjs/operators';
import { ISasToken } from './azure-storage/azureStorage';
import { BlobStorageService } from './azure-storage/blob-storage.service';
import { TableComponent } from './table/table.component';

interface IUploadProgress {
  filename: string;
  progress: number;
}

@Component({
  selector: 'app-root',
  template:`<div class="container" >
  <div style="text-align:center">
    <h4>
        Serverless Audit - Import Trial Balance
    </h4>
  </div>
  <p>
    <b>Firm ID:</b>{{this.blobStorage.firmId}}
  </p>
  <p>
    <b>Engagement ID:</b>{{this.blobStorage.engagementId}}
  </p>
  <div>
  <input type="file" (change)="onFileChange($event)" />
  </div>
  <div *ngIf="filesSelected">
    <b>Upload Progress</b> 
    <pre>{{uploadProgress$ | async | json}}</pre>
  </div>

  
  <div *ngIf="filesUploaded">
    <h4>Trial Balance</h4>
    <app-table></app-table>
</div>


  </div>
  `,
  styles: []
})
export class AppComponent {
  uploadProgress$: Observable<IUploadProgress[]>;
  filesSelected = false;
  filesUploaded = false;
  

  constructor(private blobStorage: BlobStorageService) {}

  onFileChange(event: any): void {
    this.filesSelected = true;

    this.uploadProgress$ = from(event.target.files as FileList).pipe(
      map(file => this.uploadFile(file)),
      combineAll()
    );

    this.filesUploaded = true;
  }

  uploadFile(file: File): Observable<IUploadProgress> {
    const accessToken: ISasToken = {
      container: this.blobStorage.firmId,
      filename:  this.blobStorage.engagementId + '/imported/' + file.name,
      storageAccessToken:
        '?sv=2017-11-09',
      storageUri: 'https://storeless.blob.core.windows.net'
    };

    return this.blobStorage
      .uploadToBlobStorage(accessToken, file)
      .pipe(map(progress => this.mapProgress(file, progress)));
  }

  private mapProgress(file: File, progress: number): IUploadProgress {
    return {
      filename: file.name,
      progress: progress
    };
  }
}
