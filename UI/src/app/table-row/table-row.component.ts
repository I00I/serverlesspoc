import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-table-row',
  template: `
<td *ngFor="let col of columns">
  {{character[col]}}
</td>
  `,
  styles: []
})
export class TableRowComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
