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
        '?sv=2017-11-09&ss=bfqt&srt=sco&sp=rwdlacup&se=2018-12-31T13:28:26Z&st=2018-11-12T05:28:26Z&spr=https&sig=54R40OAazXG%2FPu6KiRlZ3MC%2F%2FysSHpVAgmN92vEdZqU%3D',
      storageUri: 'https://storeless.blob.core.windows.net'
    };

    this.columns = ["Number", "Name"];    
    this.accounts = this.blobStorage.getAccounts();
  }
}
