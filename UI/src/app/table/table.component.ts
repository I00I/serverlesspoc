import { Component, OnInit } from '@angular/core';
import { ISasToken } from '../azure-storage/azureStorage';
import { BlobStorageService } from '../azure-storage/blob-storage.service';

@Component({
  selector: 'app-table',
  template: `
<table class="table">
  <tr>
    <th *ngFor="let col of columns">
      {{col}}
    </th>
  </tr>
  <tr *ngFor="let account of accounts">
    <td *ngFor="let col of columns">
      {{account[col]}}
    </td>
  </tr>
</table>
  `,
  styles: []
})
export class TableComponent implements OnInit {
    columns: any;
    accounts;

  constructor(private blobStorage: BlobStorageService) {}

  ngOnInit() {
    const accessToken: ISasToken = {
      container: this.blobStorage.firmId,
      filename:  this.blobStorage.engagementId + '/imported/' + 'import.xlsx',
      storageAccessToken:
        '?sv=2017-11-09',
      storageUri: 'https://storeless.blob.core.windows.net'
    };

    this.columns = ["Number", "Name"];    
    this.accounts = this.blobStorage.getAccounts();
  }
}
