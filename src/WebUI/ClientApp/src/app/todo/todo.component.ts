import { Component, TemplateRef, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import {
  TodoItemsClient, TodoListsClient, TodoListDto, TodoItemDto, TagDto, TagsVm,
  PriorityLevelDto, CreateTodoListCommand, UpdateTodoListCommand,
  CreateTodoItemCommand, UpdateTodoItemDetailCommand, TodosVm,
  TagsClient, CreateTagCommand
} from '../web-api-client';

@Component({
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  debug = false;
  deleting = false;
  deleteCountDown = 0;
  deleteCountDownInterval: any;
  lists: TodoListDto[];
  priorityLevels: PriorityLevelDto[];
  selectedList: TodoListDto;
  selectedItem: TodoItemDto;
  newListEditor: any = {};
  listOptionsEditor: any = {};
  newListModalRef: BsModalRef;
  listOptionsModalRef: BsModalRef;
  deleteListModalRef: BsModalRef;
  itemDetailsModalRef: BsModalRef;
  tagModalRef: BsModalRef;
  
  tags: TagDto[] = [];
  filteredTags: TagDto[] = [];
  selectedTags: TagDto[] = [];
  newTagEditor: any = {};
  searchText: string = '';
  
  get popularTags(): TagDto[] {
    return [...this.tags].sort((a, b) => b.usageCount - a.usageCount).slice(0, 3);
  }
  
  itemDetailsFormGroup = this.fb.group({
    id: [null],
    listId: [null],
    priority: [''],
    note: [''],
    tags: [[]]
  });


  constructor(
    private listsClient: TodoListsClient,
    private itemsClient: TodoItemsClient,
    private tagsClient: TagsClient,
    private modalService: BsModalService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.listsClient.get().subscribe(
      result => {
        this.lists = result.lists;
        this.priorityLevels = result.priorityLevels;
        if (this.lists.length) {
          this.selectedList = this.lists[0];
        }
      },
      error => console.error(error)
    );
    
    this.loadTags();
  }
  
  loadTags(): void {
    this.tagsClient.getTags().subscribe(
      (result: TagsVm) => {
        this.tags = result.tags;
        this.filteredTags = [...this.tags];
      },
      error => console.error(error)
    );
  }
  
  createTag(tagName: string, tagColor: string): void {
    const command = new CreateTagCommand({
      name: tagName,
      color: tagColor
    });
    
    this.tagsClient.create(command).subscribe(
      () => {
        this.loadTags();
        this.newTagEditor = {};
      },
      error => console.error(error)
    );
  }
  
  addTagToTodoItem(todoItemId: number, tagId: number): void {
    this.tagsClient.addTagToTodoItem(todoItemId, tagId).subscribe(
      () => {
        this.loadTodoItem(todoItemId);
      },
      error => console.error(error)
    );
  }
  
  removeTagFromTodoItem(todoItemId: number, tagId: number): void {
    this.tagsClient.removeTagFromTodoItem(todoItemId, tagId).subscribe(
      () => {
        this.loadTodoItem(todoItemId);
      },
      error => console.error(error)
    );
  }
  
  loadTodoItem(todoItemId: number): void {
    const list = this.lists.find(l => l.items.some(i => i.id === todoItemId));
    if (list) {
      const itemIndex = list.items.findIndex(i => i.id === todoItemId);
      if (itemIndex >= 0) {
        this.itemsClient.getTodoItemsWithPagination(1, 10, todoItemId).subscribe(
          result => {
            if (result.items.length > 0) {
              list.items[itemIndex] = result.items[0];
              if (this.selectedItem && this.selectedItem.id === todoItemId) {
                this.selectedItem = list.items[itemIndex];
              }
            }
          },
          error => console.error(error)
        );
      }
    }
  }
  
  showTagManagementModal(template: TemplateRef<any>): void {
    this.tagModalRef = this.modalService.show(template);
  }
  
  deleteTag(tagId: number): void {
    this.tagsClient.delete(tagId).subscribe(
      () => {
        this.loadTags();
      },
      error => console.error(error)
    );
  }
  
  filterByTag(tag: TagDto): void {
    const tagIndex = this.selectedTags.findIndex(t => t.id === tag.id);
    if (tagIndex >= 0) {
      this.selectedTags.splice(tagIndex, 1);
    } else {
      this.selectedTags.push(tag);
    }
    
    this.applyFilters();
  }
  
  isTagSelected(tagId: number): boolean {
    return this.selectedTags.some(t => t.id === tagId);
  }
  
  searchItems(searchText: string): void {
    this.searchText = searchText;
    this.applyFilters();
  }
  
  clearFilters(): void {
    this.selectedTags = [];
    this.searchText = '';
    this.applyFilters();
  }
  
  applyFilters(): void {
    if (!this.selectedList) {
      return;
    }

    if (this.selectedTags.length === 0 && !this.searchText) {
      this.listsClient.get().subscribe(
        result => {
          this.lists = result.lists;
          const originalList = this.lists.find(l => l.id === this.selectedList?.id);
          if (originalList) {
            this.selectedList = originalList;
          } else if (this.lists.length) {
            this.selectedList = this.lists[0];
          }
        },
        error => console.error(error)
      );
      return;
    }

    this.listsClient.get().subscribe(
      result => {
        this.lists = result.lists;
        
        const originalList = this.lists.find(l => l.id === this.selectedList?.id);
        if (!originalList) {
          return;
        }
        
        const filteredList = JSON.parse(JSON.stringify(originalList)) as TodoListDto;
        
        let filteredItems = [...filteredList.items];
        
        if (this.selectedTags.length > 0) {
          const tagIds = this.selectedTags.map(t => t.id);
          
          filteredItems = filteredItems.filter(item => {
            if (!item.tags || item.tags.length === 0) {
              return false;
            }
            
            return tagIds.some(tagId => 
              item.tags.some(itemTag => itemTag.id === tagId)
            );
          });
        }
        
        if (this.searchText && this.searchText.trim() !== '') {
          const searchLower = this.searchText.toLowerCase().trim();
          
          filteredItems = filteredItems.filter(item => 
            item.title.toLowerCase().includes(searchLower)
          );
        }
        
        filteredList.items = filteredItems;
        
        this.selectedList = filteredList;
      },
      error => console.error(error)
    );
  }
  

  // Lists
  remainingItems(list: TodoListDto): number {
    return list.items.filter(t => !t.done).length;
  }

  showNewListModal(template: TemplateRef<any>): void {
    this.newListModalRef = this.modalService.show(template);
    setTimeout(() => document.getElementById('title').focus(), 250);
  }

  newListCancelled(): void {
    this.newListModalRef.hide();
    this.newListEditor = {};
  }

  addList(): void {
    const list = {
      id: 0,
      title: this.newListEditor.title,
      items: []
    } as TodoListDto;

    this.listsClient.create(list as CreateTodoListCommand).subscribe(
      result => {
        list.id = result;
        this.lists.push(list);
        this.selectedList = list;
        this.newListModalRef.hide();
        this.newListEditor = {};
      },
      error => {
        const errors = JSON.parse(error.response);

        if (errors && errors.Title) {
          this.newListEditor.error = errors.Title[0];
        }

        setTimeout(() => document.getElementById('title').focus(), 250);
      }
    );
  }

  showListOptionsModal(template: TemplateRef<any>) {
    this.listOptionsEditor = {
      id: this.selectedList.id,
      title: this.selectedList.title
    };

    this.listOptionsModalRef = this.modalService.show(template);
  }

  updateListOptions() {
    const list = this.listOptionsEditor as UpdateTodoListCommand;
    this.listsClient.update(this.selectedList.id, list).subscribe(
      () => {
        (this.selectedList.title = this.listOptionsEditor.title),
          this.listOptionsModalRef.hide();
        this.listOptionsEditor = {};
      },
      error => console.error(error)
    );
  }

  confirmDeleteList(template: TemplateRef<any>) {
    this.listOptionsModalRef.hide();
    this.deleteListModalRef = this.modalService.show(template);
  }

  deleteListConfirmed(): void {
    this.listsClient.delete(this.selectedList.id).subscribe(
      () => {
        this.deleteListModalRef.hide();
        this.lists = this.lists.filter(t => t.id !== this.selectedList.id);
        this.selectedList = this.lists.length ? this.lists[0] : null;
      },
      error => console.error(error)
    );
  }

  // Items
  showItemDetailsModal(template: TemplateRef<any>, item: TodoItemDto): void {
    this.selectedItem = item;
    this.itemDetailsFormGroup.patchValue(this.selectedItem);

    this.itemDetailsModalRef = this.modalService.show(template);
    this.itemDetailsModalRef.onHidden.subscribe(() => {
        this.stopDeleteCountDown();
    });
  }

  updateItemDetails(): void {
    const item = new UpdateTodoItemDetailCommand(this.itemDetailsFormGroup.value);
    this.itemsClient.updateItemDetails(this.selectedItem.id, item).subscribe(
      () => {
        if (this.selectedItem.listId !== item.listId) {
          this.selectedList.items = this.selectedList.items.filter(
            i => i.id !== this.selectedItem.id
          );
          const listIndex = this.lists.findIndex(
            l => l.id === item.listId
          );
          this.selectedItem.listId = item.listId;
          this.lists[listIndex].items.push(this.selectedItem);
        }

        this.selectedItem.priority = item.priority;
        this.selectedItem.note = item.note;
        this.itemDetailsModalRef.hide();
        this.itemDetailsFormGroup.reset();
      },
      error => console.error(error)
    );
  }

  addItem() {
    const item = {
      id: 0,
      listId: this.selectedList.id,
      priority: this.priorityLevels[0].value,
      title: '',
      done: false
    } as TodoItemDto;

    this.selectedList.items.push(item);
    const index = this.selectedList.items.length - 1;
    this.editItem(item, 'itemTitle' + index);
  }

  editItem(item: TodoItemDto, inputId: string): void {
    this.selectedItem = item;
    setTimeout(() => document.getElementById(inputId).focus(), 100);
  }

  updateItem(item: TodoItemDto, pressedEnter: boolean = false): void {
    const isNewItem = item.id === 0;

    if (!item.title.trim()) {
      this.deleteItem(item);
      return;
    }

    if (item.id === 0) {
      this.itemsClient
        .create({
          ...item, listId: this.selectedList.id
        } as CreateTodoItemCommand)
        .subscribe(
          result => {
            item.id = result;
          },
          error => console.error(error)
        );
    } else {
      this.itemsClient.update(item.id, item).subscribe(
        () => console.log('Update succeeded.'),
        error => console.error(error)
      );
    }

    this.selectedItem = null;

    if (isNewItem && pressedEnter) {
      setTimeout(() => this.addItem(), 250);
    }
  }

  deleteItem(item: TodoItemDto, countDown?: boolean) {
    if (countDown) {
      if (this.deleting) {
        this.stopDeleteCountDown();
        return;
      }
      this.deleteCountDown = 3;
      this.deleting = true;
      this.deleteCountDownInterval = setInterval(() => {
        if (this.deleting && --this.deleteCountDown <= 0) {
          this.deleteItem(item, false);
        }
      }, 1000);
      return;
    }
    this.deleting = false;
    if (this.itemDetailsModalRef) {
      this.itemDetailsModalRef.hide();
    }

    if (item.id === 0) {
      const itemIndex = this.selectedList.items.indexOf(this.selectedItem);
      this.selectedList.items.splice(itemIndex, 1);
    } else {
      this.itemsClient.delete(item.id).subscribe(
        () =>
        (this.selectedList.items = this.selectedList.items.filter(
          t => t.id !== item.id
        )),
        error => console.error(error)
      );
    }
  }

  stopDeleteCountDown() {
    clearInterval(this.deleteCountDownInterval);
    this.deleteCountDown = 0;
    this.deleting = false;
  }
}
